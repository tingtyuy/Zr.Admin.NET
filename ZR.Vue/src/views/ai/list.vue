<template>
  <div class="app-container">
    <el-form :model="queryParams" ref="queryForm" :inline="true" label-width="68px">
      <el-form-item label="提示词" prop="prompt">
        <el-input v-model="queryParams.prompt" placeholder="模糊搜索提示词" clearable size="small" @keyup.enter.native="handleQuery" />
      </el-form-item>
      <el-form-item label="状态" prop="status">
        <el-select v-model="queryParams.status" placeholder="全部状态" clearable size="small">
          <el-option label="排队中" value="pending" />
          <el-option label="处理中" value="processing" />
          <el-option label="已完成" value="done" />
          <el-option label="失败" value="failed" />
        </el-select>
      </el-form-item>
      <el-form-item label="类型" prop="funcType">
        <el-select v-model="queryParams.funcType" placeholder="全部类型" clearable size="small">
          <el-option label="图生图" value="img2img" />
          <el-option label="文生图" value="txt2img" />
        </el-select>
      </el-form-item>
      <el-form-item>
        <el-button type="primary" icon="el-icon-search" size="mini" @click="handleQuery">搜索</el-button>
        <el-button icon="el-icon-refresh" size="mini" @click="resetQuery">重置</el-button>
        <el-button type="warning" icon="el-icon-refresh-right" size="mini" @click="handleBatchRetry">一键重试失败任务</el-button>
      </el-form-item>
    </el-form>

    <el-table v-loading="loading" :data="taskList" border stripe>
      <el-table-column label="任务号" prop="id" width="200" :show-overflow-tooltip="true" />
      <el-table-column label="类型" prop="funcType" width="90" align="center">
        <template slot-scope="scope">
          <el-tag size="small">{{ funcTypeMap[scope.row.funcType] || scope.row.funcType }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column label="提示词" prop="prompt" :show-overflow-tooltip="true" />
      <el-table-column label="状态" width="90" align="center">
        <template slot-scope="scope">
          <el-tag :type="statusTagType(scope.row.status)" size="small">{{ statusText[scope.row.status] }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column label="原图" width="80" align="center">
        <template slot-scope="scope">
          <el-image v-if="scope.row.inputImagePath" :src="scope.row.inputImagePath" :preview-src-list="[scope.row.inputImagePath]" fit="cover" class="thumb-img" />
        </template>
      </el-table-column>
      <el-table-column label="结果图" width="80" align="center">
        <template slot-scope="scope">
          <el-image v-if="scope.row.outputImagePath" :src="scope.row.outputImagePath" :preview-src-list="[scope.row.outputImagePath]" fit="cover" class="thumb-img" />
          <span v-else class="no-img">-</span>
        </template>
      </el-table-column>
      <el-table-column label="对比" width="70" align="center">
        <template slot-scope="scope">
          <el-button v-if="scope.row.outputImagePath" type="text" icon="el-icon-picture" @click="handleCompare(scope.row)">对比</el-button>
          <span v-else class="no-img">-</span>
        </template>
      </el-table-column>
      <el-table-column label="提交时间" prop="createTime" width="160" align="center" />
      <el-table-column label="操作" width="260" align="center" fixed="right">
        <template slot-scope="scope">
          <el-button type="text" icon="el-icon-view" @click="handleDetail(scope.row)">详情</el-button>
          <el-button v-if="scope.row.status !== 'done'" type="text" icon="el-icon-edit" @click="handleEdit(scope.row)">编辑</el-button>
          <el-button type="text" icon="el-icon-document-copy" @click="handleExtract(scope.row)">提取模板</el-button>
          <el-button type="text" icon="el-icon-refresh" @click="handleRetry(scope.row)">重试</el-button>
          <el-button v-if="scope.row.status !== 'processing'" type="text" icon="el-icon-delete" style="color:#F56C6C" @click="handleDelete(scope.row)">删除</el-button>
        </template>
      </el-table-column>
    </el-table>

    <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum" :limit.sync="queryParams.pageSize" @pagination="getList" />

    <el-dialog :visible.sync="compareVisible" :title="'对比 - ' + compareRow.prompt" width="90%" top="3vh" custom-class="compare-dialog" @close="compareVisible = false">
      <div class="compare-container">
        <div class="compare-side">
          <div class="compare-label">原图</div>
          <img :src="compareRow.inputImagePath" class="compare-img" />
        </div>
        <div class="compare-side">
          <div class="compare-label compare-label-right">结果图</div>
          <img :src="compareRow.outputImagePath" class="compare-img" />
        </div>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import { getTaskList, retryTask, deleteTask, batchRetryFailed } from '@/api/ai/task'

export default {
  name: 'AiList',
  data() {
    return {
      taskList: [],
      loading: false,
      total: 0,
      queryParams: { pageNum: 1, pageSize: 20, prompt: '', status: '', funcType: '' },
      statusText: { pending: '排队中', processing: '处理中', done: '已完成', failed: '失败' },
      funcTypeMap: { img2img: '图生图', txt2img: '文生图' },
      compareVisible: false,
      compareRow: { inputImagePath: '', outputImagePath: '', prompt: '' }
    }
  },
  created() { this.getList() },
  methods: {
    getList() {
      this.loading = true
      getTaskList(this.queryParams).then(res => {
        this.taskList = res.data.result
        this.total = res.data.totalNum
      }).finally(() => { this.loading = false })
    },
    handleQuery() { this.queryParams.pageNum = 1; this.getList() },
    resetQuery() { this.queryParams = { pageNum: 1, pageSize: 20, prompt: '', status: '', funcType: '' }; this.getList() },
    handleDetail(row) { this.$router.push({ path: '/ai/result/' + row.id }) },
    handleEdit(row) { this.$router.push({ path: '/ai/edit/' + row.id }) },
    handleExtract(row) {
      this.$router.push({ path: '/ai/submit', query: { prompt: row.prompt, name: '从任务提取' } })
    },
    handleRetry(row) {
      this.$confirm('确定重试?', '提示', { type: 'warning' }).then(() => {
        retryTask(row.id).then(() => { this.$message.success('已提交'); this.getList() })
      }).catch(() => {})
    },
    handleBatchRetry() {
      this.$confirm('确定一键重试所有失败任务?', '提示', { type: 'warning' }).then(() => {
        this.loading = true
        batchRetryFailed().then(res => {
          this.$message.success(res.data.message)
          this.getList()
        }).catch(() => {}).finally(() => { this.loading = false })
      }).catch(() => {})
    },
    handleDelete(row) {
      this.$confirm('确定删除该任务?文件将一并删除。', '警告', { type: 'warning' }).then(() => {
        deleteTask(row.id).then(() => { this.$message.success('已删除'); this.getList() })
      }).catch(() => {})
    },
    statusTagType(s) { return { pending: 'info', processing: '', done: 'success', failed: 'danger' }[s] || 'info' },
    getPreviewList(row) {
      const list = [row.inputImagePath]
      if (row.outputImagePath) list.push(row.outputImagePath)
      return list
    },
    handleCompare(row) {
      this.compareRow = row
      this.compareVisible = true
    }
  }
}
</script>

<style scoped>
.thumb-img { width: 50px; height: 50px; border-radius: 4px; }
.no-img { color: #c0c4cc; }
</style>
<style>
.compare-dialog { margin: 0 auto; }
.compare-dialog .el-dialog__body { padding: 10px 20px; }
.compare-container { display: flex; gap: 16px; height: calc(100vh - 120px); }
.compare-side { flex: 1; display: flex; flex-direction: column; align-items: center; background: #000; border-radius: 6px; overflow: hidden; }
.compare-label { padding: 8px 0; color: #909399; font-size: 14px; font-weight: bold; background: #1a1a1a; width: 100%; text-align: center; }
.compare-label-right { color: #409eff; }
.compare-img { max-width: 100%; max-height: calc(100vh - 180px); object-fit: contain; }
</style>
