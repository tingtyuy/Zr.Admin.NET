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
      </el-form-item>
    </el-form>

    <el-row :gutter="10" class="mb8">
      <el-col :span="1.5">
        <el-button type="primary" plain icon="el-icon-plus" size="mini" :disabled="multiple" @click="handleBatchTag">批量添加标签</el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button type="success" plain icon="el-icon-download" size="mini" :disabled="multiple" @click="handleBatchDownload">批量下载结果图</el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button type="warning" plain icon="el-icon-refresh-right" size="mini" @click="handleBatchRetry">一键重试失败任务</el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button type="success" plain icon="el-icon-check" size="mini" :disabled="multiple" @click="handleBatchPublish">标记为已发布</el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button type="danger" plain icon="el-icon-delete" size="mini" :disabled="multiple" @click="handleBatchDelete">批量删除</el-button>
      </el-col>
      <el-col :span="1.5">
        <el-dropdown @command="handleQuickSelect" size="mini">
          <el-button icon="el-icon-check" size="mini">快速选择<i class="el-icon-arrow-down el-icon--right"></i></el-button>
          <el-dropdown-menu slot="dropdown">
            <el-dropdown-item command="all">全选当前页</el-dropdown-item>
            <el-dropdown-item command="allPages">全选所有页</el-dropdown-item>
            <el-dropdown-item command="done">选择所有已完成</el-dropdown-item>
            <el-dropdown-item command="failed">选择所有失败</el-dropdown-item>
            <el-dropdown-item command="pending">选择所有排队中</el-dropdown-item>
            <el-dropdown-item command="unpublished">选择所有未发布</el-dropdown-item>
            <el-dropdown-item command="clear" divided>取消全选</el-dropdown-item>
          </el-dropdown-menu>
        </el-dropdown>
      </el-col>
    </el-row>

    <el-table ref="taskList" v-loading="loading" :data="taskList" border stripe @selection-change="handleSelectionChange" @row-click="handleRowClick">
      <el-table-column type="selection" width="55" align="center" :selectable="canSelect" />
      <el-table-column label="任务号" prop="id" width="200" :show-overflow-tooltip="true" />
      <el-table-column label="任务名称" prop="taskName" width="120" align="center">
        <template slot-scope="scope">
          <div class="editable-cell" @dblclick="startEdit(scope.row, 'taskName')">
            <template v-if="editingRow === scope.row.id && editingField === 'taskName'">
              <el-input v-model="editingValue" size="mini" autofocus placeholder="设置名称" @blur="saveEdit(scope.row)" @keyup.enter.native="saveEdit(scope.row)" />
            </template>
            <template v-else>
              <el-tag v-if="scope.row.taskName" size="small" type="success" class="copyable-tag" @click.native="copyText(scope.row.taskName)">{{ scope.row.taskName }}</el-tag>
              <span v-else class="no-edit-text" title="双击设置名称">点击设置</span>
              <i class="el-icon-edit-outline copy-icon" @click.stop="startEdit(scope.row, 'taskName')" title="编辑"></i>
            </template>
          </div>
        </template>
      </el-table-column>
      <el-table-column label="类型" prop="funcType" width="90" align="center">
        <template slot-scope="scope">
          <el-tag size="small">{{ funcTypeMap[scope.row.funcType] || scope.row.funcType }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column label="标签" width="180">
        <template slot-scope="scope">
          <template v-if="scope.row.tags">
            <el-tag v-for="(tag, idx) in parseTags(scope.row.tags)" :key="idx" size="mini" class="clickable-tag" @click="handleTagClick(tag)">{{ tag }}</el-tag>
          </template>
          <span v-else class="no-img">-</span>
        </template>
      </el-table-column>
      <el-table-column label="提示词" prop="prompt" min-width="200">
        <template slot-scope="scope">
          <div class="editable-cell" @dblclick="startEdit(scope.row, 'prompt')">
            <template v-if="editingRow === scope.row.id && editingField === 'prompt'">
              <el-input v-model="editingValue" size="mini" type="textarea" :rows="2" autofocus @blur="saveEdit(scope.row)" @keyup.enter.ctrl.native="saveEdit(scope.row)" />
            </template>
            <template v-else>
              <span class="prompt-text" :title="scope.row.prompt">{{ scope.row.prompt }}</span>
              <i class="el-icon-copy-document copy-icon" @click.stop="copyText(scope.row.prompt)" title="复制"></i>
            </template>
          </div>
        </template>
      </el-table-column>
      <el-table-column label="状态" width="90" align="center">
        <template slot-scope="scope">
          <el-tag :type="statusTagType(scope.row.status)" size="small">{{ statusText[scope.row.status] }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column label="发布状态" width="90" align="center">
        <template slot-scope="scope">
          <el-tag v-if="scope.row.publishStatus === 1" type="success" size="small">已发布</el-tag>
          <el-tag v-else type="info" size="small">未发布</el-tag>
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

    <!-- 对比弹窗 -->
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

    <!-- 批量管理标签弹窗 -->
    <el-dialog title="管理标签" :visible.sync="tagDialogVisible" width="520px" @close="resetTagDialog">
      <div style="margin-bottom: 12px; color: #909399;">已选择 {{ ids.length }} 个任务</div>

      <div v-if="existingTags.length" style="margin-bottom: 16px;">
        <div style="font-weight: bold; margin-bottom: 8px; color: #606266;">当前标签（点击 × 移除）</div>
        <div class="tag-list">
          <el-tag v-for="(tag, idx) in existingTags" :key="'exist-'+idx" closable size="small" type="info" @close="removeExistingTag(idx)">{{ tag }}</el-tag>
        </div>
      </div>

      <div>
        <div style="font-weight: bold; margin-bottom: 8px; color: #606266;">添加新标签</div>
        <div class="tag-input-row">
          <el-input v-model="newTagInput" placeholder="输入标签名称" size="small" style="flex:1" @keyup.enter.native="addTagToList" />
          <el-button size="small" icon="el-icon-plus" @click="addTagToList">添加</el-button>
        </div>
        <div v-if="tagList.length" class="tag-list" style="margin-top: 8px;">
          <el-tag v-for="(tag, idx) in tagList" :key="'new-'+idx" closable size="small" type="success" @close="removeTagFromList(idx)">{{ tag }}</el-tag>
        </div>
      </div>

      <span slot="footer">
        <el-button @click="tagDialogVisible = false">取消</el-button>
        <el-button type="primary" :disabled="tagList.length === 0 && removedTags.length === 0" :loading="tagSubmitting" @click="confirmBatchTag">确定</el-button>
      </span>
    </el-dialog>

    <!-- 存储路径提示 -->
    <el-dialog title="结果图存储路径" :visible.sync="pathDialogVisible" width="500px">
      <el-alert type="info" :closable="false" show-icon>
        <div slot="title" style="font-weight: bold;">ZIP下载说明</div>
        <div>下载的ZIP文件中，结果图按标签名分文件夹存储：</div>
        <div style="margin-top: 8px; font-family: monospace; background: #f5f7fa; padding: 8px; border-radius: 4px;">
          ai_results.zip<br/>
          ├── 标签A/<br/>
          │   ├── 任务号1.png<br/>
          │   └── 任务号2.png<br/>
          └── 未分类/<br/>
              └── 任务号3.png
        </div>
      </el-alert>
      <span slot="footer">
        <el-button type="primary" @click="pathDialogVisible = false">知道了</el-button>
      </span>
    </el-dialog>
  </div>
</template>

<script>
import { getTaskList, retryTask, deleteTask, batchRetryFailed, batchAddTags, batchDownloadResult, batchMarkPublished } from '@/api/ai/task'

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
      compareRow: { inputImagePath: '', outputImagePath: '', prompt: '' },
      // 多选
      ids: [],
      single: true,
      multiple: true,
      lastClickedIndex: -1,
      // 批量标签
      tagDialogVisible: false,
      newTagInput: '',
      tagList: [],
      existingTags: [],
      removedTags: [],
      tagSubmitting: false,
      // 存储路径
      pathDialogVisible: false,
      // 快捷编辑
      editingRow: null,
      editingField: '',
      editingValue: ''
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
    handleCompare(row) {
      this.compareRow = row
      this.compareVisible = true
    },
    handleTagClick(tag) {
      this.queryParams.tag = tag.trim()
      this.handleQuery()
    },
    parseTags(tagsStr) {
      if (!tagsStr) return []
      const tags = []
      const parts = tagsStr.split(',')
      let i = 0
      while (i < parts.length) {
        const part = parts[i].trim()
        if (/^\d{4}-\d{2}-\d{2}$/.test(part) && i + 1 < parts.length && !/^\d{4}-\d{2}-\d{2}$/.test(parts[i + 1].trim())) {
          tags.push(`${part},${parts[i + 1].trim()}`)
          i += 2
        } else {
          tags.push(part)
          i++
        }
      }
      return tags
    },
    // 复制文本
    copyText(text) {
      if (!text) return
      navigator.clipboard.writeText(text).then(() => {
        this.$message.success('已复制')
      }).catch(() => {
        // 降级方案
        const textarea = document.createElement('textarea')
        textarea.value = text
        document.body.appendChild(textarea)
        textarea.select()
        document.execCommand('copy')
        document.body.removeChild(textarea)
        this.$message.success('已复制')
      })
    },
    // 开始编辑
    startEdit(row, field) {
      this.editingRow = row.id
      this.editingField = field
      this.editingValue = row[field] || ''
    },
    // 保存编辑
    saveEdit(row) {
      const newValue = this.editingValue.trim()
      const oldValue = row[this.editingField] || ''
      if (newValue !== oldValue) {
        const data = {}
        data[this.editingField] = newValue || null
        import('@/api/ai/task').then(({ updateTask }) => {
          updateTask(row.id, data).then(() => {
            this.$set(row, this.editingField, newValue)
            this.$message.success('修改成功')
          }).catch(() => {
            this.$message.error('修改失败')
          })
        })
      }
      this.editingRow = null
      this.editingField = ''
      this.editingValue = ''
    },
    // 多选
    handleSelectionChange(selection) {
      this.ids = selection.map(item => item.id)
      this.single = selection.length != 1
      this.multiple = !selection.length
    },
    handleRowClick(row, column, event) {
      const currentIndex = this.taskList.findIndex(item => item.id === row.id)
      if (event.shiftKey && this.lastClickedIndex >= 0 && this.lastClickedIndex !== currentIndex) {
        // Shift+点击：选中范围内的所有行
        const start = Math.min(this.lastClickedIndex, currentIndex)
        const end = Math.max(this.lastClickedIndex, currentIndex)
        for (let i = start; i <= end; i++) {
          if (!this.ids.includes(this.taskList[i].id)) {
            this.$refs.taskList.toggleRowSelection(this.taskList[i], true)
          }
        }
      }
      this.lastClickedIndex = currentIndex
    },
    canSelect(row) {
      return true
    },
    // 批量管理标签
    handleBatchTag() {
      if (this.ids.length === 0) {
        this.$message.warning('请先选择任务')
        return
      }
      // 收集选中任务的所有现有标签（去重）
      const tagSet = new Set()
      this.taskList.forEach(row => {
        if (this.ids.includes(row.id) && row.tags) {
          this.parseTags(row.tags).forEach(t => tagSet.add(t))
        }
      })
      this.existingTags = [...tagSet]
      this.removedTags = []
      this.tagDialogVisible = true
    },
    addTagToList() {
      const tag = this.newTagInput.trim()
      if (tag && !this.tagList.includes(tag)) {
        this.tagList.push(tag)
        this.newTagInput = ''
      }
    },
    removeTagFromList(idx) {
      this.tagList.splice(idx, 1)
    },
    removeExistingTag(idx) {
      const tag = this.existingTags[idx]
      this.existingTags.splice(idx, 1)
      if (!this.removedTags.includes(tag)) {
        this.removedTags.push(tag)
      }
    },
    resetTagDialog() {
      this.newTagInput = ''
      this.tagList = []
      this.existingTags = []
      this.removedTags = []
      this.tagSubmitting = false
    },
    confirmBatchTag() {
      if (this.tagList.length === 0 && this.removedTags.length === 0) {
        this.$message.warning('请至少添加或删除一个标签')
        return
      }
      this.tagSubmitting = true
      batchAddTags(this.ids, this.tagList.join(','), this.removedTags.join(',')).then(res => {
        this.$message.success(res.data.message)
        this.tagDialogVisible = false
        this.getList()
      }).catch(() => {
        this.$message.error('操作失败')
      }).finally(() => {
        this.tagSubmitting = false
      })
    },
    // 批量标记已发布
    handleBatchPublish() {
      if (this.ids.length === 0) {
        this.$message.warning('请先选择任务')
        return
      }
      this.$confirm(`确定将选中的 ${this.ids.length} 个任务标记为已发布？`, '确认', { type: 'warning' }).then(() => {
        this.loading = true
        batchMarkPublished(this.ids).then(res => {
          this.$message.success(res.data.message)
          this.getList()
        }).catch(() => {
          this.$message.error('操作失败')
        }).finally(() => {
          this.loading = false
        })
      }).catch(() => {})
    },
    // 批量删除
    handleBatchDelete() {
      if (this.ids.length === 0) {
        this.$message.warning('请先选择任务')
        return
      }
      this.$confirm(`确定删除选中的 ${this.ids.length} 个任务？文件将一并删除，不可恢复！`, '警告', { type: 'warning', confirmButtonText: '确定删除', confirmButtonClass: 'el-button--danger' }).then(() => {
        this.loading = true
        let deleted = 0
        const total = this.ids.length
        const deleteNext = (index) => {
          if (index >= this.ids.length) {
            this.$message.success(`成功删除 ${deleted} 个任务`)
            this.getList()
            this.loading = false
            return
          }
          deleteTask(this.ids[index]).then(() => {
            deleted++
            deleteNext(index + 1)
          }).catch(() => {
            deleteNext(index + 1)
          })
        }
        deleteNext(0)
      }).catch(() => {})
    },
    // 快速选择
    handleQuickSelect(command) {
      if (command === 'clear') {
        this.$refs.taskList.clearSelection()
        return
      }
      // 全选所有页：需要从后端获取所有ID
      if (command === 'allPages') {
        this.loading = true
        getTaskList({ pageNum: 1, pageSize: 9999 }).then(res => {
          const allIds = (res.data.result || []).map(item => item.id)
          this.ids = allIds
          // 选中当前页中匹配的行
          this.$nextTick(() => {
            this.taskList.forEach(row => {
              if (this.ids.includes(row.id)) {
                this.$refs.taskList.toggleRowSelection(row, true)
              }
            })
          })
          this.$message.success(`已选择全部 ${allIds.length} 个任务`)
        }).catch(() => { this.$message.error('获取任务列表失败') }).finally(() => { this.loading = false })
        return
      }
      // 先清空当前选择
      this.$refs.taskList.clearSelection()
      // 根据条件筛选并选中
      this.$nextTick(() => {
        this.taskList.forEach(row => {
          let shouldSelect = false
          switch (command) {
            case 'all':
              shouldSelect = true
              break
            case 'done':
              shouldSelect = row.status === 'done'
              break
            case 'failed':
              shouldSelect = row.status === 'failed'
              break
            case 'pending':
              shouldSelect = row.status === 'pending'
              break
            case 'unpublished':
              shouldSelect = row.publishStatus !== 1
              break
          }
          if (shouldSelect) {
            this.$refs.taskList.toggleRowSelection(row, true)
          }
        })
      })
    },
    // 批量下载
    handleBatchDownload() {
      if (this.ids.length === 0) {
        this.$message.warning('请先选择任务')
        return
      }
      this.$confirm(`确定下载选中任务的结果图？将按标签名分文件夹打包为ZIP。`, '提示', { type: 'info' }).then(() => {
        this.loading = true
        batchDownloadResult(this.ids).then(res => {
          const blob = new Blob([res.data], { type: 'application/zip' })
          const url = window.URL.createObjectURL(blob)
          const link = document.createElement('a')
          link.href = url
          link.download = 'ai_results.zip'
          document.body.appendChild(link)
          link.click()
          document.body.removeChild(link)
          window.URL.revokeObjectURL(url)
          this.$message.success('下载成功')
          this.pathDialogVisible = true
        }).catch(() => {
          this.$message.error('下载失败')
        }).finally(() => {
          this.loading = false
        })
      }).catch(() => {})
    }
  }
}
</script>

<style scoped>
.thumb-img { width: 50px; height: 50px; border-radius: 4px; }
.no-img { color: #c0c4cc; }
.clickable-tag { cursor: pointer; margin-right: 4px; margin-bottom: 2px; }
.clickable-tag:hover { opacity: 0.8; }
.mb8 { margin-bottom: 12px; }
.tag-input-row { display: flex; gap: 8px; align-items: center; }
.tag-list { display: flex; gap: 6px; flex-wrap: wrap; }
.editable-cell { display: flex; align-items: center; gap: 4px; }
.editable-cell:hover .copy-icon { opacity: 1; }
.copy-icon { opacity: 0; cursor: pointer; color: #409EFF; font-size: 14px; transition: opacity 0.2s; }
.copy-icon:hover { color: #337ab7; }
.copyable-tag { cursor: pointer; }
.no-edit-text { color: #c0c4cc; font-size: 12px; cursor: pointer; }
.no-edit-text:hover { color: #409EFF; }
.prompt-text { overflow: hidden; text-overflow: ellipsis; white-space: nowrap; display: inline-block; max-width: 200px; vertical-align: middle; }
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
