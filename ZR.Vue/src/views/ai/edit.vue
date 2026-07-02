<template>
  <div class="app-container">
    <el-card v-if="taskInfo" class="edit-card">
      <div slot="header" class="clearfix">
        <span>编辑任务</span>
        <el-tag :type="statusType" style="margin-left: 10px">{{ statusText }}</el-tag>
      </div>

      <el-form ref="editForm" :model="editForm" :rules="rules" label-width="100px">
        <el-form-item label="任务号">
          <el-input v-model="taskInfo.taskNo" disabled />
        </el-form-item>
        <el-form-item label="状态">
          <el-input :value="statusText" disabled />
        </el-form-item>
        <el-form-item label="提示词" prop="prompt">
          <el-input v-model="editForm.prompt" type="textarea" :rows="4" placeholder="请输入提示词" />
        </el-form-item>
        <el-form-item label="原图">
          <el-image
            v-if="taskInfo.inputImageUrl"
            :src="taskInfo.inputImageUrl"
            fit="contain"
            class="preview-image"
          />
          <span v-else class="no-image">无原图</span>
        </el-form-item>
        <el-form-item label="结果图" v-if="taskInfo.outputImageUrl">
          <el-image
            :src="taskInfo.outputImageUrl"
            fit="contain"
            class="preview-image"
          />
        </el-form-item>
      </el-form>

      <div class="form-footer">
        <el-button @click="handleCancel">取消</el-button>
        <el-button v-if="canEdit" type="primary" :loading="submitting" @click="handleSubmit">保存</el-button>
        <el-button v-if="taskInfo.status === 'failed'" type="warning" @click="handleRetry">重试任务</el-button>
      </div>
    </el-card>

    <el-card v-else-if="!loading">
      <el-empty description="任务不存在" />
    </el-card>
  </div>
</template>

<script>
import { getTaskStatus, updateTask, retryTask } from '@/api/ai/task'

export default {
  name: 'AiEdit',
  data() {
    return {
      taskNo: '',
      taskInfo: null,
      editForm: { prompt: '' },
      rules: {
        prompt: [{ required: true, message: '请输入提示词', trigger: 'blur' }]
      },
      loading: false,
      submitting: false
    }
  },
  computed: {
    canEdit() {
      return this.taskInfo && this.taskInfo.status !== 'done'
    },
    statusType() {
      const map = { pending: 'info', processing: '', done: 'success', failed: 'danger' }
      return map[this.taskInfo?.status] || 'info'
    },
    statusText() {
      const map = { pending: '排队中', processing: '处理中', done: '已完成', failed: '失败' }
      return map[this.taskInfo?.status] || '未知'
    }
  },
  created() {
    this.taskNo = this.$route.params.taskNo
    if (this.taskNo) {
      this.loadTask()
    }
  },
  methods: {
    loadTask() {
      this.loading = true
      getTaskStatus(this.taskNo).then(response => {
        this.taskInfo = response.data
        this.editForm.prompt = this.taskInfo.prompt || ''
      }).catch(() => {
        this.$message.error('加载任务失败')
      }).finally(() => {
        this.loading = false
      })
    },
    handleSubmit() {
      this.$refs.editForm.validate(valid => {
        if (!valid) return
        this.submitting = true
        updateTask(this.taskNo, { prompt: this.editForm.prompt }).then(() => {
          this.$message.success('保存成功')
          this.loadTask()
        }).catch(() => {
          this.$message.error('保存失败')
        }).finally(() => {
          this.submitting = false
        })
      })
    },
    handleRetry() {
      this.$confirm('确定重试该任务?', '提示', { type: 'warning' }).then(() => {
        retryTask(this.taskNo).then(() => {
          this.$message.success('已提交重试')
          this.loadTask()
        })
      }).catch(() => {})
    },
    handleCancel() {
      this.$router.go(-1)
    }
  }
}
</script>

<style scoped>
.edit-card {
  max-width: 700px;
  margin: 0 auto;
}
.preview-image {
  max-width: 100%;
  max-height: 250px;
  border-radius: 4px;
  border: 1px solid #ebeef5;
}
.no-image {
  color: #909399;
}
.form-footer {
  text-align: center;
  padding-top: 20px;
  border-top: 1px solid #ebeef5;
  margin-top: 20px;
}
</style>
