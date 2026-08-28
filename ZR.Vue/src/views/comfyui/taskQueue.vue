<template>
  <div class="app-container">
    <el-card class="box-card">
      <div slot="header" class="clearfix">
        <span>ComfyUI 执行队列</span>
        <span style="float:right;font-size:12px;color:#909399">已入队的任务，由后台Worker自动提交至ComfyUI执行</span>
      </div>

      <el-form :model="queryParams" :inline="true" label-width="68px">
        <el-form-item label="状态" prop="status">
          <el-select v-model="queryParams.status" placeholder="全部状态" clearable size="small">
            <el-option label="待执行" value="pending" />
            <el-option label="执行中" value="processing" />
            <el-option label="已完成" value="done" />
            <el-option label="失败" value="failed" />
            <el-option label="已取消" value="cancelled" />
          </el-select>
        </el-form-item>
        <el-form-item label="类型" prop="funcType">
          <el-select v-model="queryParams.funcType" placeholder="全部类型" clearable size="small">
            <el-option label="文生图" value="txt2img" />
            <el-option label="图生图" value="img2img" />
            <el-option label="文生视频" value="txt2video" />
            <el-option label="图生视频" value="img2video" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" icon="el-icon-search" size="mini" @click="handleQuery">搜索</el-button>
          <el-button icon="el-icon-refresh" size="mini" @click="resetQuery">重置</el-button>
          <el-button icon="el-icon-refresh-right" size="mini" @click="getList">刷新</el-button>
        </el-form-item>
      </el-form>

      <el-table v-loading="loading" :data="queueList" border stripe>
        <el-table-column label="任务名" prop="taskName" min-width="160" :show-overflow-tooltip="true" />
        <el-table-column label="类型" prop="funcType" width="90" align="center">
          <template slot-scope="scope">
            <el-tag size="small">{{ funcTypeText[scope.row.funcType] || scope.row.funcType }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column label="状态" width="100" align="center">
          <template slot-scope="scope">
            <el-tag size="small" :type="statusTagType(scope.row.status)">{{ statusText[scope.row.status] || scope.row.status }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column label="进度" width="150" align="center">
          <template slot-scope="scope">
            <el-progress :percentage="scope.row.progress || 0" :status="progressStatus(scope.row.status)" :stroke-width="10" />
          </template>
        </el-table-column>
        <el-table-column label="输出" min-width="160" align="center">
          <template slot-scope="scope">
            <div v-if="outputs(scope.row).length > 0" class="output-list">
              <template v-for="(o, i) in outputs(scope.row).slice(0, 3)">
                <video v-if="o.type === 'video'" :key="'v' + i" :src="o.url" class="output-thumb" @click="previewOutputs(scope.row)" />
                <img v-else :key="'i' + i" :src="o.url" class="output-thumb" @click="previewOutputs(scope.row)" />
              </template>
              <el-button v-if="outputs(scope.row).length > 3" size="mini" type="text" @click="previewOutputs(scope.row)">
                +{{ outputs(scope.row).length - 3 }}
              </el-button>
            </div>
            <span v-else-if="scope.row.status === 'done'" style="color:#909399">无输出</span>
            <span v-else style="color:#909399">-</span>
          </template>
        </el-table-column>
        <el-table-column label="错误信息" min-width="200">
          <template slot-scope="scope">
            <template v-if="scope.row.errorMessage">
              <span class="error-text" :title="scope.row.errorMessage" style="color:#F56C6C">{{ scope.row.errorMessage }}</span>
              <i class="el-icon-copy-document copy-btn" title="复制错误信息" @click="copyText(scope.row.errorMessage)" />
            </template>
            <span v-else>-</span>
          </template>
        </el-table-column>
        <el-table-column label="入队时间" prop="queuedTime" width="150" align="center" />
        <el-table-column label="操作" width="120" align="center" fixed="right">
          <template slot-scope="scope">
            <el-button v-if="scope.row.status === 'pending'" type="text" icon="el-icon-close" style="color:#F56C6C" @click="handleCancel(scope.row)">取消</el-button>
            <el-button v-if="scope.row.status === 'done' || scope.row.status === 'failed'" type="text" icon="el-icon-back" @click="handleDequeue(scope.row)">出队</el-button>
          </template>
        </el-table-column>
      </el-table>

      <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum" :limit.sync="queryParams.pageSize" @pagination="getList" />
    </el-card>

    <!-- 输出预览弹窗 -->
    <el-dialog title="输出预览" :visible.sync="previewVisible" width="min(95vw, 1100px)" top="3vh">
      <div v-if="previewList.length > 0" class="preview-container">
        <!-- 左侧文件列表 -->
        <div class="preview-sidebar-wrap">
          <div ref="previewSidebar" class="preview-sidebar">
            <div
              v-for="(item, idx) in previewList"
              :key="idx"
              class="preview-item"
              :class="{ active: idx === previewIndex }"
              @click="previewIndex = idx"
            >
              <video v-if="item.type === 'video'" :src="item.url" muted preload="metadata" class="preview-item-img" />
              <img v-else :src="item.url" class="preview-item-img" />
              <div class="preview-item-overlay">
                <i v-if="item.type === 'video'" class="el-icon-video-play"></i>
                <span class="preview-item-name">{{ item.filename || item.name || item.url.split('/').pop() || (item.type === 'video' ? '视频' : '图片') }}</span>
              </div>
            </div>
          </div>
          <div class="sidebar-nav">
            <el-tooltip content="滚动到顶部" placement="left">
              <div class="sidebar-nav-btn" @click="scrollSidebar('top')"><i class="el-icon-top"></i></div>
            </el-tooltip>
            <el-tooltip content="滚动到底部" placement="left">
              <div class="sidebar-nav-btn" @click="scrollSidebar('bottom')"><i class="el-icon-bottom"></i></div>
            </el-tooltip>
          </div>
        </div>
        <!-- 右侧预览区 -->
        <div class="preview-main">
          <div v-if="previewList[previewIndex].type === 'video'" class="preview-video-wrap">
            <video :key="'pv' + previewIndex" :src="previewList[previewIndex].url" controls autoplay class="preview-video"></video>
          </div>
          <div v-else class="preview-image-wrap">
            <img :key="'pi' + previewIndex" :src="previewList[previewIndex].url" class="preview-image" />
            <span class="preview-counter">{{ previewIndex + 1 }} / {{ previewList.length }}</span>
          </div>
        </div>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import { getComfyuiQueueList, cancelComfyuiQueue, dequeueComfyuiQueue } from '@/api/comfyui/index'

export default {
  name: 'ComfyuiQueueList',
  data() {
    return {
      queueList: [],
      loading: false,
      total: 0,
      queryParams: { pageNum: 1, pageSize: 20, status: '', funcType: '' },
      funcTypeText: { txt2img: '文生图', img2img: '图生图', txt2video: '文生视频', img2video: '图生视频' },
      statusText: { pending: '待执行', processing: '执行中', done: '已完成', failed: '失败', cancelled: '已取消' },
      previewVisible: false,
      previewList: [],
      previewIndex: 0
    }
  },
  created() { this.getList(); this.startPolling() },
  beforeDestroy() { this.stopPolling() },
  methods: {
    getList() {
      this.loading = true
      getComfyuiQueueList(this.queryParams).then(res => {
        this.queueList = res.data.result
        this.total = res.data.totalNum
      }).finally(() => { this.loading = false })
    },
    startPolling() {
      this.pollingTimer = setInterval(() => {
        if (this.isAnyRunning()) this.getList()
      }, 5000)
    },
    stopPolling() { if (this.pollingTimer) { clearInterval(this.pollingTimer); this.pollingTimer = null } },
    isAnyRunning() {
      return this.queueList.some(x => x.status === 'pending' || x.status === 'processing')
    },
    handleQuery() { this.queryParams.pageNum = 1; this.getList() },
    resetQuery() { this.queryParams = { pageNum: 1, pageSize: 20, status: '', funcType: '' }; this.getList() },
    outputs(row) {
      if (!row.outputUrls) return []
      try { return JSON.parse(row.outputUrls) } catch (e) { return [] }
    },
    previewOutputs(row) {
      const all = this.outputs(row)
      this.previewList = all.sort((a, b) => {
        if (a.type === 'video' && b.type !== 'video') return -1
        if (a.type !== 'video' && b.type === 'video') return 1
        return 0
      })
      this.previewIndex = 0
      this.previewVisible = true
    },
    scrollSidebar(target) {
      this.$nextTick(() => {
        const el = this.$refs.previewSidebar
        if (!el) return
        el.scrollTo({ top: target === 'top' ? 0 : el.scrollHeight, behavior: 'smooth' })
      })
    },
    statusTagType(status) {
      const map = { pending: 'warning', processing: '', done: 'success', failed: 'danger', cancelled: 'info' }
      return map[status] || 'info'
    },
    progressStatus(status) {
      if (status === 'failed') return 'exception'
      if (status === 'done') return 'success'
      return ''
    },
    handleCancel(row) {
      this.$confirm('确定取消该队列任务？', '警告', { type: 'warning' }).then(() => {
        cancelComfyuiQueue(row.id).then(res => {
          this.$message.success(res.data.message)
          this.getList()
        }).catch(() => { this.$message.error('取消失败') })
      }).catch(() => {})
    },
    handleDequeue(row) {
      this.$confirm('出队后该任务将回到任务列表（可重新编辑），确定？', '确认', { type: 'warning' }).then(() => {
        dequeueComfyuiQueue(row.id).then(res => {
          this.$message.success(res.data.message)
          this.getList()
        }).catch(() => { this.$message.error('出队失败') })
      }).catch(() => {})
    },
    copyText(text) {
      if (!text) return
      navigator.clipboard.writeText(text).then(() => {
        this.$message.success('已复制')
      }).catch(() => {
        const textarea = document.createElement('textarea')
        textarea.value = text
        document.body.appendChild(textarea)
        textarea.select()
        document.execCommand('copy')
        document.body.removeChild(textarea)
        this.$message.success('已复制')
      })
    }
  }
}
</script>

<style scoped>
.mb8 { margin-bottom: 12px; }
.output-list { display: flex; align-items: center; justify-content: center; gap: 4px; }
.output-thumb { width: 48px; height: 48px; object-fit: cover; border-radius: 4px; cursor: zoom-in; border: 1px solid #eee; }
.error-text {
  display: inline-block;
  max-width: 200px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  vertical-align: middle;
}
.copy-btn {
  color: #909399;
  cursor: pointer;
  margin-left: 4px;
  vertical-align: middle;
}
.copy-btn:hover { color: #409EFF; }
.preview-container {
  display: flex;
  gap: 16px;
  height: 70vh;
  overflow: hidden;
}
.preview-sidebar-wrap {
  width: 180px;
  flex-shrink: 0;
  display: flex;
  flex-direction: column;
  gap: 8px;
}
.preview-sidebar {
  flex: 1;
  overflow-y: auto;
  display: flex;
  flex-direction: column;
  gap: 8px;
  padding-right: 4px;
}
.preview-sidebar::-webkit-scrollbar { width: 6px; }
.preview-sidebar::-webkit-scrollbar-thumb { background: #dcdfe6; border-radius: 3px; }
.sidebar-nav { display: flex; justify-content: center; gap: 8px; }
.sidebar-nav-btn {
  width: 28px;
  height: 28px;
  border-radius: 50%;
  background: #f0f2f5;
  color: #606266;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all .2s;
}
.sidebar-nav-btn:hover { background: #409EFF; color: #fff; }
.preview-item {
  position: relative;
  height: 90px;
  flex-shrink: 0;
  border: 2px solid transparent;
  border-radius: 6px;
  overflow: hidden;
  cursor: pointer;
  transition: border-color .2s;
}
.preview-item:hover { border-color: #909399; }
.preview-item.active { border-color: #409EFF; }
.preview-item-img { width: 100%; height: 100%; object-fit: cover; display: block; }
.preview-item-overlay {
  position: absolute;
  left: 0;
  right: 0;
  bottom: 0;
  display: flex;
  align-items: center;
  gap: 4px;
  padding: 2px 6px;
  background: linear-gradient(transparent, rgba(0,0,0,.7));
  color: #fff;
  font-size: 12px;
}
.preview-item-name {
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  max-width: 100%;
}
.preview-main {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  min-width: 0;
  background: #f5f7fa;
  border-radius: 6px;
  overflow: hidden;
  position: relative;
}
.preview-image-wrap { position: relative; width: 100%; height: 100%; display: flex; align-items: center; justify-content: center; }
.preview-image { max-width: 100%; max-height: 100%; object-fit: contain; }
.preview-video-wrap { width: 100%; height: 100%; display: flex; align-items: center; justify-content: center; }
.preview-video { max-width: 100%; max-height: 100%; object-fit: contain; }
.preview-counter {
  position: absolute;
  bottom: 10px;
  right: 12px;
  padding: 2px 10px;
  border-radius: 10px;
  background: rgba(0,0,0,.5);
  color: #fff;
  font-size: 12px;
  z-index: 1;
}
</style>
