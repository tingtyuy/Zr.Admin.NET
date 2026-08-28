<template>
  <div class="app-container">
    <el-card class="box-card">
      <div slot="header" class="clearfix">
        <span>ComfyUI 任务列表</span>
        <span style="float:right;font-size:12px;color:#909399">所有任务先在此展示，勾选后【入队】进入ComfyUI执行队列</span>
      </div>

      <el-form :model="queryParams" :inline="true" label-width="68px">
        <el-form-item label="状态" prop="status">
          <el-select v-model="queryParams.status" placeholder="全部状态" clearable size="small">
            <el-option label="草稿" value="draft" />
            <el-option label="待执行" value="pending" />
            <el-option label="执行中" value="processing" />
            <el-option label="已完成" value="done" />
            <el-option label="失败" value="failed" />
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
        <el-form-item label="名称" prop="prompt">
          <el-input v-model="queryParams.prompt" placeholder="任务名/工作流名" clearable size="small" @keyup.enter.native="handleQuery" />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" icon="el-icon-search" size="mini" @click="handleQuery">搜索</el-button>
          <el-button icon="el-icon-refresh" size="mini" @click="resetQuery">重置</el-button>
          <el-button icon="el-icon-refresh-right" size="mini" @click="getList">刷新</el-button>
        </el-form-item>
      </el-form>

      <el-row :gutter="10" class="mb8">
        <el-col :span="1.5">
          <el-button type="primary" plain icon="el-icon-upload2" size="mini" :disabled="multiple" @click="handleEnqueue">入队执行</el-button>
        </el-col>
        <el-col :span="1.5">
          <el-button type="danger" plain icon="el-icon-delete" size="mini" :disabled="multiple" @click="handleBatchDelete">批量删除</el-button>
        </el-col>
      </el-row>

      <el-table v-loading="loading" :data="taskList" border stripe @selection-change="handleSelectionChange">
        <el-table-column type="selection" width="45" align="center" />
        <el-table-column label="任务名" prop="taskName" min-width="160" :show-overflow-tooltip="true" />
        <el-table-column label="工作流" prop="workflowName" min-width="120" :show-overflow-tooltip="true" />
        <el-table-column label="类型" prop="funcType" width="90" align="center">
          <template slot-scope="scope">
            <el-tag size="small">{{ funcTypeText[scope.row.funcType] || scope.row.funcType }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column label="状态" width="100" align="center">
          <template slot-scope="scope">
            <el-tag size="small" :type="statusTagType(scope.row)">
              {{ statusText(scope.row) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="进度" width="140" align="center">
          <template slot-scope="scope">
            <el-progress v-if="scope.row.queued === 1" :percentage="scope.row.progress || 0" :status="progressStatus(scope.row)" :stroke-width="10" />
            <span v-else style="color:#909399">未入队</span>
          </template>
        </el-table-column>
        <el-table-column label="创建时间" prop="createTime" width="150" align="center" />
        <el-table-column label="输出" min-width="160" align="center">
          <template slot-scope="scope">
            <div v-if="outputs(scope.row).length > 0" class="output-list">
              <img v-for="(o, i) in outputs(scope.row).filter(x => x.type === 'image')" :key="'i' + i" :src="o.url" class="output-thumb" @click="previewOutput(o)" />
              <el-button v-if="outputs(scope.row).filter(x => x.type === 'video').length > 0" size="mini" type="text" icon="el-icon-video-play" @click="previewOutput(outputs(scope.row).find(x => x.type === 'video'))">
                视频预览
              </el-button>
            </div>
            <span v-else-if="scope.row.queueStatus === 'done'" style="color:#909399">无输出</span>
            <span v-else style="color:#909399">-</span>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="160" align="center" fixed="right">
          <template slot-scope="scope">
            <el-button v-if="scope.row.queued === 0" type="text" icon="el-icon-upload2" @click="handleEnqueueSingle(scope.row)">入队</el-button>
            <el-button type="text" icon="el-icon-delete" style="color:#F56C6C" @click="handleDelete(scope.row)">删除</el-button>
          </template>
        </el-table-column>
      </el-table>

      <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum" :limit.sync="queryParams.pageSize" @pagination="getList" />
    </el-card>

    <!-- 输出预览弹窗 -->
    <el-dialog title="输出预览" :visible.sync="previewVisible" width="min(90vw, 800px)" top="5vh">
      <div v-if="previewItem" style="text-align:center">
        <img v-if="previewItem.type === 'image'" :src="previewItem.url" style="max-width:100%;max-height:75vh;object-fit:contain" />
        <video v-else controls :src="previewItem.url" style="max-width:100%;max-height:75vh"></video>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import { getComfyuiTaskList, deleteComfyuiTask, batchDeleteComfyuiTask, enqueueComfyuiTask } from '@/api/comfyui/index'

export default {
  name: 'ComfyuiTaskList',
  data() {
    return {
      taskList: [],
      loading: false,
      total: 0,
      queryParams: { pageNum: 1, pageSize: 20, status: '', funcType: '', prompt: '' },
      ids: [],
      multiple: true,
      funcTypeText: { txt2img: '文生图', img2img: '图生图', txt2video: '文生视频', img2video: '图生视频' },
      previewVisible: false,
      previewItem: null
    }
  },
  created() { this.getList() },
  methods: {
    getList() {
      this.loading = true
      getComfyuiTaskList(this.queryParams).then(res => {
        this.taskList = res.data.result
        this.total = res.data.totalNum
      }).finally(() => { this.loading = false })
    },
    handleQuery() { this.queryParams.pageNum = 1; this.getList() },
    resetQuery() { this.queryParams = { pageNum: 1, pageSize: 20, status: '', funcType: '', prompt: '' }; this.getList() },
    handleSelectionChange(selection) {
      this.ids = selection.filter(x => x.queued === 0).map(item => item.id)
      this.multiple = !this.ids.length
    },
    statusText(row) {
      if (row.queued === 1 && row.queueStatus) {
        const map = { pending: '待执行', processing: '执行中', done: '已完成', failed: '失败', cancelled: '已取消' }
        return map[row.queueStatus] || row.queueStatus
      }
      return '草稿'
    },
    statusTagType(row) {
      if (row.queued === 1 && row.queueStatus) {
        const map = { pending: 'warning', processing: '', done: 'success', failed: 'danger', cancelled: 'info' }
        return map[row.queueStatus] || 'info'
      }
      return 'info'
    },
    progressStatus(row) {
      if (row.queueStatus === 'failed') return 'exception'
      if (row.queueStatus === 'done') return 'success'
      return ''
    },
    outputs(row) {
      if (!row.outputUrls) return []
      try { return JSON.parse(row.outputUrls) } catch (e) { return [] }
    },
    previewOutput(item) {
      this.previewItem = item
      this.previewVisible = true
    },
    handleEnqueue() {
      if (!this.ids.length) { this.$message.warning('请选择草稿任务'); return }
      this.$confirm(`确定将 ${this.ids.length} 个任务加入ComfyUI执行队列？`, '确认', { type: 'warning' }).then(() => {
        enqueueComfyuiTask(this.ids).then(res => {
          this.$message.success(res.data.message)
          this.getList()
        }).catch(() => { this.$message.error('入队失败') })
      }).catch(() => {})
    },
    handleEnqueueSingle(row) {
      this.$confirm(`确定将任务「${row.taskName}」加入执行队列？`, '确认', { type: 'warning' }).then(() => {
        enqueueComfyuiTask([row.id]).then(res => {
          this.$message.success(res.data.message)
          this.getList()
        }).catch(() => { this.$message.error('入队失败') })
      }).catch(() => {})
    },
    handleDelete(row) {
      this.$confirm('确定删除该任务？', '警告', { type: 'warning' }).then(() => {
        deleteComfyuiTask(row.id).then(res => {
          this.$message.success(res.data.message || '已删除')
          this.getList()
        }).catch(err => { this.$message.error(err.msg || '删除失败') })
      }).catch(() => {})
    },
    handleBatchDelete() {
      if (!this.ids.length) { this.$message.warning('请选择任务'); return }
      this.$confirm(`确定删除选中的 ${this.ids.length} 个任务？`, '警告', { type: 'warning' }).then(() => {
        batchDeleteComfyuiTask(this.ids).then(res => {
          this.$message.success(res.data.message)
          this.getList()
        }).catch(() => { this.$message.error('删除失败') })
      }).catch(() => {})
    }
  }
}
</script>

<style scoped>
.mb8 { margin-bottom: 12px; }
.output-list { display: flex; align-items: center; justify-content: center; gap: 4px; }
.output-thumb { width: 48px; height: 48px; object-fit: cover; border-radius: 4px; cursor: zoom-in; border: 1px solid #eee; }
</style>
