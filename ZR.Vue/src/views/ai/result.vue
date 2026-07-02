<template>
  <div class="app-container">
    <el-card class="box-card">
      <div slot="header" class="clearfix">
        <span>AI 任务查询</span>
      </div>

      <el-form :inline="true" :model="queryForm" class="query-form">
        <el-form-item label="任务号">
          <el-input v-model="queryForm.taskNo" placeholder="请输入任务号" clearable />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleQuery" :loading="loading">查询</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <el-card v-if="taskInfo" class="result-card">
      <div slot="header" class="clearfix">
        <span>任务详情</span>
        <el-tag :type="statusType" style="margin-left: 10px">{{ statusText }}</el-tag>
      </div>

      <el-descriptions :column="2" border>
        <el-descriptions-item label="任务号">{{ taskInfo.taskNo }}</el-descriptions-item>
        <el-descriptions-item label="状态">{{ statusText }}</el-descriptions-item>
        <el-descriptions-item label="提示词" :span="2">{{ taskInfo.prompt }}</el-descriptions-item>
        <el-descriptions-item label="提交时间">{{ taskInfo.createTime }}</el-descriptions-item>
        <el-descriptions-item label="完成时间">{{ taskInfo.completeTime || '-' }}</el-descriptions-item>
      </el-descriptions>

      <el-row :gutter="20" style="margin-top: 20px">
        <el-col :span="12">
          <div class="image-box">
            <div class="image-title">原图</div>
            <el-image
              v-if="taskInfo.inputImageUrl"
              :src="taskInfo.inputImageUrl"
              :preview-src-list="[taskInfo.inputImageUrl]"
              fit="contain"
              class="result-image"
            >
              <div slot="error" class="image-error">
                <i class="el-icon-picture-outline"></i>
                <span>图片加载失败</span>
              </div>
            </el-image>
          </div>
        </el-col>
        <el-col :span="12">
          <div class="image-box">
            <div class="image-title">结果图</div>
            <el-image
              v-if="taskInfo.outputImageUrl"
              :src="taskInfo.outputImageUrl"
              :preview-src-list="[taskInfo.outputImageUrl]"
              fit="contain"
              class="result-image"
            >
              <div slot="error" class="image-error">
                <i class="el-icon-picture-outline"></i>
                <span>图片加载失败</span>
              </div>
            </el-image>
            <div v-else class="no-result">
              <i class="el-icon-loading" v-if="taskInfo.status === 'processing'"></i>
              <i class="el-icon-time" v-else-if="taskInfo.status === 'pending'"></i>
              <i class="el-icon-warning" v-else></i>
              <span v-if="taskInfo.status === 'processing'">处理中，请稍后...</span>
              <span v-else-if="taskInfo.status === 'pending'">排队中...</span>
              <span v-else-if="taskInfo.status === 'failed'">{{ taskInfo.errorMessage || '处理失败' }}</span>
              <span v-else>暂无结果</span>
            </div>
          </div>
        </el-col>
      </el-row>

      <div style="margin-top: 15px; text-align: center">
        <el-button v-if="taskInfo.status === 'done' && taskInfo.outputImageUrl" type="primary" icon="el-icon-download" @click="handleDownload">下载结果图</el-button>
        <el-button v-if="taskInfo.status === 'failed'" type="warning" icon="el-icon-refresh" @click="handleRetry">重试</el-button>
      </div>
    </el-card>
  </div>
</template>

<script>
import { getTaskStatus, retryTask } from '@/api/ai/task'

export default {
  name: 'AiResult',
  data() {
    return {
      queryForm: {
        taskNo: ''
      },
      taskInfo: null,
      loading: false,
      refreshTimer: null
    }
  },
  computed: {
    statusType() {
      const map = {
        pending: 'info',
        processing: '',
        done: 'success',
        failed: 'danger'
      }
      return map[this.taskInfo?.status] || 'info'
    },
    statusText() {
      const map = {
        pending: '排队中',
        processing: '处理中',
        done: '已完成',
        failed: '失败'
      }
      return map[this.taskInfo?.status] || '未知'
    }
  },
  mounted() {
    // 从路由参数或query中获取任务号
    const taskNo = this.$route.params.taskNo || this.$route.query.taskNo
    if (taskNo) {
      this.queryForm.taskNo = taskNo
      this.handleQuery()
    }
  },
  beforeDestroy() {
    this.stopAutoRefresh()
  },
  methods: {
    handleQuery() {
      if (!this.queryForm.taskNo) {
        this.$message.warning('请输入任务号')
        return
      }

      this.loading = true
      getTaskStatus(this.queryForm.taskNo).then(response => {
        this.taskInfo = response.data
        this.startAutoRefresh()
      }).catch(() => {
        this.$message.error('查询失败')
        this.taskInfo = null
      }).finally(() => {
        this.loading = false
      })
    },
    startAutoRefresh() {
      this.stopAutoRefresh()
      // 如果任务还在处理中，自动刷新
      if (this.taskInfo && (this.taskInfo.status === 'pending' || this.taskInfo.status === 'processing')) {
        this.refreshTimer = setInterval(() => {
          this.autoRefresh()
        }, 5000)
      }
    },
    stopAutoRefresh() {
      if (this.refreshTimer) {
        clearInterval(this.refreshTimer)
        this.refreshTimer = null
      }
    },
    autoRefresh() {
      if (!this.queryForm.taskNo) return
      getTaskStatus(this.queryForm.taskNo).then(response => {
        this.taskInfo = response.data
        if (this.taskInfo.status === 'done' || this.taskInfo.status === 'failed') {
          this.stopAutoRefresh()
          if (this.taskInfo.status === 'done') {
            this.$message.success('任务已完成')
          }
        }
      }).catch(() => {
        this.stopAutoRefresh()
      })
    },
    handleDownload() {
      if (this.taskInfo.outputImageUrl) {
        window.open(this.taskInfo.outputImageUrl, '_blank')
      }
    },
    handleRetry() {
      retryTask(this.taskInfo.taskNo).then(() => {
        this.$message.success('重试任务已提交')
        this.handleQuery()
      }).catch(() => {
        this.$message.error('重试失败')
      })
    }
  }
}
</script>

<style scoped>
.query-form {
  margin-bottom: 0;
}
.result-card {
  margin-top: 20px;
}
.image-box {
  border: 1px solid #ebeef5;
  border-radius: 4px;
  padding: 10px;
  text-align: center;
  background: #fafafa;
}
.image-title {
  font-weight: bold;
  margin-bottom: 8px;
  color: #606266;
  font-size: 14px;
}
.result-image {
  max-width: 100%;
  max-height: 320px;
  object-fit: contain;
  border-radius: 4px;
}
.no-result {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 200px;
  color: #909399;
}
.no-result i {
  font-size: 36px;
  margin-bottom: 8px;
}
.image-error {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 200px;
  color: #909399;
}
.image-error i {
  font-size: 36px;
  margin-bottom: 8px;
}
</style>
