<template>
  <div class="app-container">
    <el-card class="box-card">
      <div slot="header" class="clearfix">
        <span>ComfyUI {{ title }}</span>
        <span style="float: right; font-size: 12px; color: #909399">创建工作流任务（草稿，可在任务列表中入队执行）</span>
      </div>

      <el-form ref="form" :model="form" :rules="rules" label-width="100px" class="form-left">
        <el-form-item label="选择工作流" prop="workflowId">
          <el-select v-model="form.workflowId" placeholder="先选择工作流" filterable style="width:100%" @change="handleWorkflowChange">
            <el-option v-for="w in workflows" :key="w.id" :label="w.name" :value="w.id">
              <span>{{ w.name }}</span>
              <span style="float:right;color:#909399;font-size:12px">{{ w.nodeCount }}个节点</span>
            </el-option>
          </el-select>
        </el-form-item>

        <!-- 动态显示可变节点（一个节点生成一个对应类型的输入元素） -->
        <template v-if="variables.length > 0">
          <el-form-item v-for="v in variables" :key="v.nodeId" :label="v.label || v.nodeId" :required="isFileNode(v)">
            <el-input v-if="v.type === 'prompt' || v.type === 'value'"
              v-model="variablesValue[v.nodeId]" :type="v.type === 'value' ? 'input' : 'textarea'"
              :rows="v.type === 'value' ? 1 : 3"
              :placeholder="placeholderText(v)" />
            <el-input-number v-else-if="v.type === 'number'"
              v-model="variablesValue[v.nodeId]" :min="0" size="small" />
            <el-switch v-else-if="v.type === 'bool'"
              v-model="variablesValue[v.nodeId]"
              active-text="是" inactive-text="否"
              :active-value="true" :inactive-value="false" />
            <el-upload v-else-if="isFileNode(v)"
              class="upload-btn"
              action="#"
              :auto-upload="false"
              :limit="1"
              :accept="v.type === 'video' ? '.mp4,.webm,.mov,.avi' : '.png,.jpg,.jpeg,.webp,.gif,.bmp'"
              :on-change="(file) => handleFileChange(v.nodeId, file)"
              :on-remove="() => handleFileRemove(v.nodeId)"
              :file-list="fileListMap[v.nodeId] || []">
              <el-button size="small" type="primary" icon="el-icon-upload">{{ v.type === 'video' ? '选择参考视频' : '选择参考图' }}</el-button>
              <div slot="tip" class="el-upload__tip">上传后会自动同步到ComfyUI的input目录</div>
            </el-upload>
            <div v-if="isFileNode(v) && fileMap[v.nodeId]" style="color:#67C23A;font-size:12px;margin-top:4px">
              已选择: {{ (fileMap[v.nodeId].name) }}
            </div>
          </el-form-item>
        </template>
        <el-alert v-else-if="form.workflowId" type="warning" :closable="false" style="margin-bottom:16px"
          title="该工作流未配置可变节点，将直接按工作流原JSON执行。如需替换提示词/参考图，请到【工作流管理】配置可变节点。" />

        <el-form-item label="生成数量">
          <el-input-number v-model="form.taskCount" :min="1" :max="20" size="small" />
          <span style="margin-left:8px;color:#909399;font-size:12px">将创建 {{ form.taskCount }} 个任务（草稿）</span>
        </el-form-item>

        <el-form-item>
          <el-button type="primary" @click="handleSubmit" :loading="loading">创建任务</el-button>
          <el-button @click="handleSaveDraft" :loading="loading" icon="el-icon-document">保存草稿</el-button>
          <el-button @click="resetForm">重置</el-button>
        </el-form-item>
      </el-form>

      <div v-if="taskResult" class="task-result">
        <el-divider content-position="left">任务已创建</el-divider>
        <el-alert v-if="!taskResult.validationError" :title="'已创建 ' + taskResult.taskNos.length + ' 个任务（草稿）'" type="success" :closable="false">
          <div slot="default">
            <p>任务已保存，可到【任务列表】查看并手动入队进入ComfyUI执行队列。</p>
            <el-button type="primary" size="small" @click="goToTaskList">去任务列表入队</el-button>
          </div>
        </el-alert>
        <el-alert v-else :title="'已保存 ' + taskResult.taskNos.length + ' 个草稿'" type="warning" :closable="false">
          <div slot="default">
            <p style="color:#E6A23C">{{ taskResult.validationError }}</p>
            <p>任务已保存为草稿，补充文件后可到【任务列表】入队执行。</p>
            <el-button type="primary" size="small" @click="goToTaskList">去任务列表</el-button>
          </div>
        </el-alert>
      </div>
    </el-card>
  </div>
</template>

<script>
import { getWorkflowList, getWorkflowVariables, createComfyuiTask } from '@/api/comfyui/index'

export default {
  name: 'ComfyuiCreateTask',
  props: {
    funcType: { type: String, required: true },
    title: { type: String, default: '' }
  },
  data() {
    return {
      form: { workflowId: '', taskCount: 1 },
      rules: {
        workflowId: [{ required: true, message: '请选择工作流', trigger: 'change' }]
      },
      loading: false,
      workflows: [],
      variables: [],
      variablesValue: {},
      fileMap: {},
      fileListMap: {},
      taskResult: null
    }
  },
  created() { this.loadWorkflows() },
  methods: {
    isFileNode(v) { return v.type === 'image' || v.type === 'video' },
    loadWorkflows() {
      getWorkflowList({ pageNum: 1, pageSize: 100, category: this.funcType }).then(res => {
        this.workflows = res.data.result || []
        if (this.workflows.length === 0) {
          this.$message.warning('当前分类下没有工作流，请先到【工作流管理】导入')
        }
      })
    },
    handleWorkflowChange(wid) {
      this.variables = []
      this.variablesValue = {}
      this.fileMap = {}
      this.fileListMap = {}
      if (!wid) return
      getWorkflowVariables(wid).then(res => {
        this.variables = (res.data || []).filter(v => v.enabled !== false)
      })
    },
    placeholderText(v) {
      if (v.type === 'prompt') return '描述你想要生成的内容...'
      return '请输入...'
    },
    handleFileChange(nodeId, file) {
      this.$set(this.fileMap, nodeId, file.raw)
      this.$set(this.fileListMap, nodeId, [file])
    },
    handleFileRemove(nodeId) {
      this.$set(this.fileMap, nodeId, null)
      this.$set(this.fileListMap, nodeId, [])
    },
    handleSubmit() {
      this.$refs.form.validate(valid => {
        if (!valid) return
        this.doCreateTask()
      })
    },
    handleSaveDraft() {
      if (!this.form.workflowId) {
        this.$message.warning('请至少选择一个工作流')
        return
      }
      this.doCreateTask()
    },
    doCreateTask() {
      this.loading = true
      const fd = new FormData()
      fd.append('workflowId', this.form.workflowId)
      fd.append('funcType', this.funcType)
      fd.append('taskCount', this.form.taskCount || 1)
      const values = {}
      this.variables.forEach(v => {
        if (this.isFileNode(v)) return
        const val = String(this.variablesValue[v.nodeId] == null ? '' : this.variablesValue[v.nodeId]).trim()
        if (val) values[v.nodeId] = val
      })
      fd.append('variableValues', JSON.stringify(values))
      this.variables.forEach(v => {
        if (this.isFileNode(v) && this.fileMap[v.nodeId]) {
          fd.append('ref_' + v.nodeId, this.fileMap[v.nodeId])
        }
      })
      createComfyuiTask(fd).then(res => {
        this.taskResult = { taskNos: res.data.taskNos || [], validationError: res.data.validationError || null }
        this.$message.success(res.data.message || '已保存为草稿')
      }).catch(err => {
        this.$message.error((err.msg) || '保存失败')
      }).finally(() => { this.loading = false })
    },
    resetForm() {
      this.form = { workflowId: '', taskCount: 1 }
      this.variables = []
      this.variablesValue = {}
      this.fileMap = {}
      this.fileListMap = {}
      this.taskResult = null
    },
    goToTaskList() {
      this.$router.push({ path: '/comfyui/task-list' })
    }
  }
}
</script>

<style scoped>
.form-left { max-width: 720px; }
.task-result { margin-top: 20px; }
.upload-btn { display: inline-block; }
</style>
