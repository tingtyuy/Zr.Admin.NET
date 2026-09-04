<template>
  <div class="app-container">
    <el-card class="box-card">
      <div slot="header" class="clearfix">
        <span>ComfyUI {{ title }}</span>
        <span style="float: right; font-size: 12px; color: #909399">创建工作流任务，创建成功后自动加入执行队列</span>
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
            <template v-if="v.type === 'prompt' || v.type === 'value'">
              <div class="input-with-switch">
                <el-input
                  v-model="variablesValue[v.nodeId]"
                  :type="v.type === 'value' ? 'input' : 'textarea'"
                  :rows="v.type === 'value' ? 1 : 3"
                  :placeholder="placeholderText(v)"
                  @input="handleTranslationInput(v)"
                />
                <el-button class="lang-switch-btn" plain size="small" @click="handleSwitchLang(v)">中英切换</el-button>
              </div>
              <div v-if="translateHint[v.nodeId]" class="translate-hint">
                <span class="hint-label">中文提示：</span>{{ translateHint[v.nodeId] }}
              </div>
            </template>
            <el-input-number
              v-else-if="v.type === 'number'"
              v-model="variablesValue[v.nodeId]"
              :min="0"
              size="small"
            />
            <el-switch
              v-else-if="v.type === 'bool'"
              v-model="variablesValue[v.nodeId]"
              active-text="是"
              inactive-text="否"
              :active-value="true"
              :inactive-value="false"
            />
            <el-upload
              v-else-if="isFileNode(v)"
              class="upload-btn"
              action="#"
              :auto-upload="false"
              :limit="1"
              :accept="v.type === 'video' ? '.mp4,.webm,.mov,.avi' : '.png,.jpg,.jpeg,.webp,.gif,.bmp'"
              :on-change="(file) => handleFileChange(v.nodeId, file)"
              :on-remove="() => handleFileRemove(v.nodeId)"
              :file-list="fileListMap[v.nodeId] || []"
            >
              <el-button size="small" type="primary" icon="el-icon-upload">{{ v.type === 'video' ? '选择参考视频' : '选择参考图' }}</el-button>
              <div slot="tip" class="el-upload__tip">上传后会自动同步到ComfyUI的input目录</div>
            </el-upload>
            <div v-if="isFileNode(v) && fileMap[v.nodeId]" style="color:#67C23A;font-size:12px;margin-top:4px">
              已选择: {{ (fileMap[v.nodeId].name) }}
            </div>
          </el-form-item>
        </template>
        <el-alert
          v-else-if="form.workflowId"
          type="warning"
          :closable="false"
          style="margin-bottom:16px"
          title="该工作流未配置可变节点，将直接按工作流原JSON执行。如需替换提示词/参考图，请到【工作流管理】配置可变节点。"
        />

        <el-form-item label="生成数量">
          <el-input-number v-model="form.taskCount" :min="1" :max="20" size="small" />
          <span style="margin-left:8px;color:#909399;font-size:12px">将创建 {{ form.taskCount }} 个任务并自动入队执行</span>
        </el-form-item>

        <el-form-item label="种子模式">
          <el-radio-group v-model="form.seedMode" size="small">
            <el-radio label="random">随机种子</el-radio>
            <el-radio label="fixed">固定种子</el-radio>
          </el-radio-group>
          <span style="margin-left:8px;color:#909399;font-size:12px">随机种子用于每次生成不同结果，固定种子用于复现工作流原结果</span>
        </el-form-item>

        <el-form-item>
          <el-button type="primary" :loading="loading" @click="handleSubmit">创建任务</el-button>
          <el-button :loading="loading" icon="el-icon-document" @click="handleSaveDraft">保存草稿</el-button>
          <el-button @click="resetForm">重置</el-button>
        </el-form-item>
      </el-form>

      <div v-if="taskResult" class="task-result">
        <el-divider content-position="left">任务已创建</el-divider>
        <el-alert v-if="!taskResult.validationError" :title="'已创建 ' + taskResult.taskNos.length + ' 个任务并入队执行'" type="success" :closable="false">
          <div slot="default">
            <p>任务已保存并自动加入ComfyUI执行队列，可到【任务列表】或【执行队列】查看进度。</p>
            <el-button type="primary" size="small" @click="goToTaskQueue">去执行队列查看</el-button>
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
import { getWorkflowList, getWorkflowVariables, createComfyuiTask, translateComfyuiText } from '@/api/comfyui/index'

export default {
  name: 'ComfyuiCreateTask',
  props: {
    funcType: { type: String, required: true },
    title: { type: String, default: '' }
  },
  data() {
    return {
      form: { workflowId: '', taskCount: 1, seedMode: 'random' },
      rules: {
        workflowId: [{ required: true, message: '请选择工作流', trigger: 'change' }]
      },
      loading: false,
      workflows: [],
      variables: [],
      variablesValue: {},
      translateHint: {},
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
        } else if (!this.form.workflowId || this.workflows.length > 1) {
          // 有工作流时默认选中第一个，避免空选项
          this.form.workflowId = this.workflows[0].id
          this.handleWorkflowChange(this.form.workflowId)
        }
      })
    },
    handleWorkflowChange(wid) {
      this.variables = []
      this.variablesValue = {}
      this.translateHint = {}
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
    handleSwitchLang(v) {
      const nodeId = v.nodeId
      const text = String(this.variablesValue[nodeId] == null ? '' : this.variablesValue[nodeId]).trim()
      if (!text) {
        this.$message.warning('请先输入内容再进行中英文切换')
        return
      }
      // 含中文则翻成英文，否则翻成中文
      const isZh = /[\u4e00-\u9fff]/.test(text)
      const target = isZh ? 'en' : 'zh-CN'
      translateComfyuiText({ text: text, target: target }).then(res => {
        const translated = res.data.translated
        if (translated && translated !== text) {
          this.$set(this.variablesValue, nodeId, translated)
          this.$message.success(target === 'zh-CN' ? '已翻译成中文' : '已翻译成英文')
        } else {
          this.$message.info('内容已是目标语言')
        }
      }).catch(err => {
        this.$message.error(err.msg || '翻译失败')
      })
    },
    handleTranslationInput(v) {
      const nodeId = v.nodeId
      clearTimeout(this._transTimer)
      this._transTimer = setTimeout(() => {
        const text = String(this.variablesValue[nodeId] == null ? '' : this.variablesValue[nodeId]).trim()
        if (!text) {
          this.$set(this.translateHint, nodeId, '')
          return
        }
        // 已是中文则无需提示
        if (/[\u4e00-\u9fff]/.test(text)) {
          this.$set(this.translateHint, nodeId, '')
          return
        }
        translateComfyuiText({ text: text, target: 'zh-CN' }).then(res => {
          const t = res.data.translated
          this.$set(this.translateHint, nodeId, (t && t !== text) ? t : '')
        }).catch(() => {})
      }, 900)
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
      fd.append('seedMode', this.form.seedMode || 'random')
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
      this.form = { workflowId: '', taskCount: 1, seedMode: 'random' }
      this.variables = []
      this.variablesValue = {}
      this.translateHint = {}
      this.fileMap = {}
      this.fileListMap = {}
      this.taskResult = null
    },
    goToTaskList() {
      this.$router.push({ path: '/comfyui/task-list' })
    },
    goToTaskQueue() {
      this.$router.push({ path: '/comfyui/task-queue' })
    }
  }
}
</script>

<style scoped>
.form-left { max-width: 720px; }
.task-result { margin-top: 20px; }
.upload-btn { display: inline-block; }
.input-with-switch { display: flex; align-items: flex-start; gap: 8px; }
.input-with-switch .el-input { flex: 1; }
.lang-switch-btn { flex-shrink: 0; }
.translate-hint { margin-top: 4px; font-size: 12px; color: #606266; background: #f5f7fa; padding: 4px 8px; border-radius: 4px; line-height: 1.5; }
.hint-label { color: #409EFF; font-weight: 600; }
</style>
