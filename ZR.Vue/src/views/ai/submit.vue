<template>
  <div class="app-container">
    <el-card class="box-card">
      <div slot="header" class="clearfix">
        <span>AI 图生图</span>
      </div>

      <div class="form-row">
        <el-form ref="form" :model="form" :rules="rules" label-width="80px" class="form-left">
          <el-form-item label="上传图片" prop="file">
            <el-upload
              class="image-uploader"
              action="#"
              :show-file-list="false"
              :before-upload="beforeUpload"
              :http-request="handleUpload"
              accept="image/*"
            >
              <img v-if="form.imageUrl" :src="form.imageUrl" class="uploaded-image" />
              <i v-else class="el-icon-plus image-uploader-icon"></i>
            </el-upload>
            <div class="el-upload__tip">只能上传图片文件，且不超过10MB</div>
          </el-form-item>

          <el-form-item label="提示词模板">
            <div style="display:flex;gap:8px;align-items:center;width:100%">
              <el-select v-model="selectedTemplateId" placeholder="选择模板快速填入" clearable size="small" style="flex:1" @change="handleTemplateChange">
                <el-option v-for="t in templates" :key="t.id" :label="t.name" :value="t.id" />
              </el-select>
              <el-button size="small" icon="el-icon-folder-checked" @click="handleSaveTemplate" :disabled="!form.prompt">保存为模板</el-button>
              <el-button v-if="selectedTemplateId" size="small" type="danger" icon="el-icon-delete" circle @click="handleDeleteTemplate" />
            </div>
          </el-form-item>

          <el-form-item label="提示词" prop="prompt">
            <el-input
              v-model="form.prompt"
              type="textarea"
              :rows="3"
              placeholder="请描述你想要的效果，例如：水彩画风格、动漫风格、油画风格..."
            />
          </el-form-item>

          <el-form-item label="任务名称">
            <div style="display:flex;gap:8px;width:100%">
              <el-select v-model="form.taskNameType" placeholder="选择名称" style="width:120px" @change="onNameTypeChange">
                <el-option label="首页" value="首页" />
                <el-option label="二图" value="二图" />
                <el-option label="三图" value="三图" />
                <el-option label="四图" value="四图" />
                <el-option label="自定义" value="custom" />
              </el-select>
              <el-input v-if="form.taskNameType === 'custom'" v-model="form.taskName" placeholder="输入自定义名称" style="flex:1" />
            </div>
          </el-form-item>

          <el-form-item label="生成数量">
            <el-input-number v-model="form.taskCount" :min="1" :max="20" size="small" />
            <span style="margin-left:8px;color:#909399;font-size:12px">
              {{ form.taskCount > 1 && form.taskNameType !== 'custom' ? `将生成 ${form.taskCount} 个任务：${form.taskName}1 ~ ${form.taskName}${form.taskCount}` : '' }}
            </span>
          </el-form-item>

          <el-form-item>
            <el-button type="primary" @click="handleSubmit" :loading="loading">提交任务</el-button>
            <el-button @click="resetForm">重置</el-button>
          </el-form-item>
        </el-form>

        <div v-if="form.imageUrl" class="preview-right">
          <img :src="form.imageUrl" class="large-preview-img" />
        </div>
      </div>

      <div v-if="taskResult" class="task-result">
        <el-divider content-position="left">任务已提交</el-divider>
        <el-alert
          :title="'任务号: ' + taskResult.taskNo"
          type="success"
          show-icon
          :closable="false"
        >
          <div slot="default">
            <p>请保存好您的任务号，用于查询结果。</p>
            <el-button type="primary" size="small" @click="goToQuery">去查询结果</el-button>
          </div>
        </el-alert>
      </div>
    </el-card>

    <el-dialog title="保存提示词模板" :visible.sync="saveDialogVisible" width="400px">
      <el-form :model="saveForm" label-width="80px">
        <el-form-item label="模板名称">
          <el-input v-model="saveForm.name" placeholder="给模板起个名字" />
        </el-form-item>
        <el-form-item label="提示词">
          <el-input v-model="saveForm.prompt" type="textarea" :rows="3" readonly />
        </el-form-item>
      </el-form>
      <span slot="footer">
        <el-button @click="saveDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="confirmSaveTemplate">确定</el-button>
      </span>
    </el-dialog>
  </div>
</template>

<script>
import { submitTask, getTemplateList, saveTemplate, deleteTemplate } from '@/api/ai/task'

export default {
  name: 'AiSubmit',
  data() {
    return {
      form: { prompt: '', file: null, imageUrl: '', taskNameType: '', taskName: '', taskCount: 1 },
      rules: { prompt: [{ required: true, message: '请输入提示词', trigger: 'blur' }] },
      loading: false,
      taskResult: null,
      templates: [],
      selectedTemplateId: '',
      saveDialogVisible: false,
      saveForm: { name: '', prompt: '' }
    }
  },
  created() {
    this.loadTemplates()
    // 支持从任务列表"提取为模板"跳转过来
    if (this.$route.query.prompt) {
      this.form.prompt = this.$route.query.prompt
    }
    if (this.$route.query.name) {
      this.saveForm.name = this.$route.query.name
      this.saveForm.prompt = this.$route.query.prompt
      this.saveDialogVisible = true
    }
  },
  methods: {
    loadTemplates() {
      getTemplateList().then(res => { this.templates = res.data || [] })
    },
    handleTemplateChange(id) {
      if (!id) return
      const t = this.templates.find(x => x.id === id)
      if (t) this.form.prompt = t.prompt
    },
    handleSaveTemplate() {
      this.saveForm = { name: '', prompt: this.form.prompt }
      this.saveDialogVisible = true
    },
    confirmSaveTemplate() {
      if (!this.saveForm.name) { this.$message.warning('请输入模板名称'); return }
      saveTemplate({ name: this.saveForm.name, prompt: this.saveForm.prompt, funcType: 'img2img' }).then(() => {
        this.$message.success('保存成功')
        this.saveDialogVisible = false
        this.loadTemplates()
      })
    },
    handleDeleteTemplate() {
      if (!this.selectedTemplateId) return
      this.$confirm('确定删除该模板?', '提示', { type: 'warning' }).then(() => {
        deleteTemplate(this.selectedTemplateId).then(() => {
          this.$message.success('已删除')
          this.selectedTemplateId = ''
          this.loadTemplates()
        })
      }).catch(() => {})
    },
    beforeUpload(file) {
      if (!file.type.startsWith('image/')) { this.$message.error('只能上传图片!'); return false }
      if (file.size / 1024 / 1024 > 10) { this.$message.error('不能超过10MB!'); return false }
      this.form.imageUrl = URL.createObjectURL(file)
      this.form.file = file
      return false
    },
    handleUpload() {},
    onNameTypeChange(val) {
      if (val !== 'custom') {
        this.form.taskName = val
      } else {
        this.form.taskName = ''
      }
    },
    handleSubmit() {
      this.$refs.form.validate(valid => {
        if (!valid) return
        if (!this.form.file) { this.$message.error('请先上传图片'); return }
        this.loading = true
        const fd = new FormData()
        fd.append('prompt', this.form.prompt)
        fd.append('file', this.form.file)
        fd.append('taskName', this.form.taskName || '')
        fd.append('taskCount', this.form.taskCount || 1)
        submitTask(fd).then(res => {
          this.taskResult = { taskNo: res.data.taskNos ? res.data.taskNos.join(', ') : res.data.taskNo }
          const count = res.data.taskNos ? res.data.taskNos.length : 1
          this.$message.success(`成功提交 ${count} 个任务`)
        }).catch(() => { this.$message.error('提交失败') }).finally(() => { this.loading = false })
      })
    },
    resetForm() {
      this.form = { prompt: '', file: null, imageUrl: '', taskNameType: '', taskName: '', taskCount: 1 }
      this.selectedTemplateId = ''
      this.taskResult = null
    },
    goToQuery() {
      this.$router.push({ path: '/ai/result/' + this.taskResult.taskNo })
    }
  }
}
</script>

<style scoped>
.form-row { display: flex; gap: 24px; align-items: flex-start; }
.form-left { flex: 1; min-width: 0; }
.image-uploader { border: 1px dashed #d9d9d9; border-radius: 6px; cursor: pointer; width: 178px; height: 178px; }
.image-uploader:hover { border-color: #409EFF; }
.image-uploader-icon { font-size: 28px; color: #8c939d; width: 178px; height: 178px; line-height: 178px; text-align: center; }
.uploaded-image { width: 178px; height: 178px; display: block; object-fit: contain; }
.preview-right { flex: 1; min-width: 0; max-height: calc(100vh - 220px); border: 1px solid #ebeef5; border-radius: 8px; background: #fafafa; display: flex; align-items: center; justify-content: center; padding: 8px; }
.large-preview-img { max-width: 100%; max-height: calc(100vh - 220px); display: block; object-fit: contain; }
.task-result { margin-top: 20px; }
</style>
