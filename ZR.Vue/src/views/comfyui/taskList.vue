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
              <template v-for="(o, i) in outputs(scope.row).slice(0, 3)">
                <video v-if="o.type === 'video'" :key="'v' + i" :src="o.url" muted preload="metadata" class="output-thumb" @click="previewOutputs(scope.row)" />
                <img v-else :key="'i' + i" :src="o.url" class="output-thumb" @click="previewOutputs(scope.row)" />
              </template>
              <el-button v-if="outputs(scope.row).length > 3" size="mini" type="text" @click="previewOutputs(scope.row)">
                +{{ outputs(scope.row).length - 3 }}
              </el-button>
            </div>
            <span v-else-if="scope.row.queueStatus === 'done'" style="color:#909399">无输出</span>
            <span v-else style="color:#909399">-</span>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="220" align="center" fixed="right">
          <template slot-scope="scope">
            <el-button type="text" icon="el-icon-view" @click="openDetail(scope.row, 'view')">查看</el-button>
            <el-button type="text" icon="el-icon-edit" @click="openDetail(scope.row, 'edit')">编辑</el-button>
            <el-button v-if="scope.row.queueStatus !== 'pending' && scope.row.queueStatus !== 'processing'" type="text" icon="el-icon-upload2" @click="handleEnqueueSingle(scope.row)">入队</el-button>
            <el-button type="text" icon="el-icon-delete" style="color:#F56C6C" @click="handleDelete(scope.row)">删除</el-button>
          </template>
        </el-table-column>
      </el-table>

      <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum" :limit.sync="queryParams.pageSize" @pagination="getList" />
    </el-card>

    <!-- 任务明细 / 编辑弹窗 -->
    <el-dialog :title="detailDialog.mode === 'edit' ? '编辑任务明细' : '任务明细'" :visible.sync="detailVisible" width="min(95vw, 820px)" top="5vh">
      <div v-if="detailData" v-loading="detailLoading">
        <!-- 基本信息 -->
        <el-descriptions :column="2" border size="mini">
          <el-descriptions-item label="任务名">{{ detailData.taskName }}</el-descriptions-item>
          <el-descriptions-item label="工作流">{{ detailData.workflowName }}</el-descriptions-item>
          <el-descriptions-item label="类型">{{ funcTypeText[detailData.funcType] || detailData.funcType }}</el-descriptions-item>
          <el-descriptions-item label="状态">
            <el-tag size="small" :type="statusTagType(rowOfDetail)">{{ statusText(rowOfDetail) }}</el-tag>
          </el-descriptions-item>
          <el-descriptions-item label="创建时间">{{ detailData.createTime }}</el-descriptions-item>
          <el-descriptions-item label="入队时间">{{ detailData.queuedTime || '-' }}</el-descriptions-item>
          <el-descriptions-item v-if="detailErrorText" label="错误信息" :span="2">
            <span style="color:#F56C6C">{{ detailErrorText }}</span>
          </el-descriptions-item>
        </el-descriptions>

        <!-- 编辑模式：变量/参考文件表单 -->
        <template v-if="detailDialog.mode === 'edit'">
          <el-divider content-position="left">编辑输入</el-divider>
          <div v-if="detailDialog.variables.length === 0" style="color:#909399;font-size:12px">
            该工作流未配置可变节点，无需填写。保存后直接入队按工作流原JSON执行。
          </div>
          <el-form v-else ref="detailForm" :model="detailDialog.values" label-width="100px">
            <el-form-item v-for="v in detailDialog.variables" :key="v.nodeId" :label="v.label || v.nodeId" :required="isFileNode(v)">
              <template v-if="v.type === 'prompt' || v.type === 'value'">
                <div class="input-with-switch">
                  <el-input
                    v-model="detailDialog.values[v.nodeId]" :type="v.type === 'value' ? 'input' : 'textarea'"
                    :rows="v.type === 'value' ? 1 : 3" :placeholder="v.type === 'prompt' ? '描述你想要生成的内容...' : '请输入...'"
                    @input="handleTranslationInput(v)" />
                  <el-button class="lang-switch-btn" plain size="small" @click="handleSwitchLang(v)">中英切换</el-button>
                </div>
                <div v-if="detailDialog.translateHint[v.nodeId]" class="translate-hint">
                  <span class="hint-label">中文提示：</span>{{ detailDialog.translateHint[v.nodeId] }}
                </div>
              </template>
              <el-input-number v-else-if="v.type === 'number'" v-model="detailDialog.values[v.nodeId]" :min="0" size="small" />
              <el-switch v-else-if="v.type === 'bool'" v-model="detailDialog.values[v.nodeId]"
                active-text="是" inactive-text="否" :active-value="true" :inactive-value="false" />
              <el-upload v-else-if="isFileNode(v)" class="upload-btn"
                action="#" :auto-upload="false" :limit="1"
                :accept="v.type === 'video' ? '.mp4,.webm,.mov,.avi' : '.png,.jpg,.jpeg,.webp,.gif,.bmp'"
                :on-change="(file) => handleEditFileChange(v.nodeId, file)"
                :on-remove="() => handleEditFileRemove(v.nodeId)"
                :file-list="fileListMap[v.nodeId] || []">
                <el-button size="small" type="primary" icon="el-icon-upload">{{ v.type === 'video' ? '替换参考视频' : '替换参考图' }}</el-button>
                <div slot="tip" class="el-upload__tip">
                  <span v-if="fileOfNode(v)" style="color:#67C23A">当前文件：{{ fileOfNode(v).originalName }}</span>
                  <span v-else style="color:#E6A23C">尚未上传</span>
                  <span v-if="fileMap[v.nodeId]" style="color:#409EFF;margin-left:6px">已选择新文件：{{ fileMap[v.nodeId].name }}</span>
                </div>
              </el-upload>
            </el-form-item>
          </el-form>
        </template>

        <!-- 查看模式：变量值/参考文件 -->
        <template v-else>
          <template v-if="detailDialog.variables.length > 0">
            <el-divider content-position="left">输入内容</el-divider>
            <el-descriptions :column="1" border size="mini">
              <el-descriptions-item v-for="v in detailDialog.variables" :key="v.nodeId" :label="(v.label || v.nodeId)">
                <template v-if="isFileNode(v)">
                  <template v-if="fileOfNode(v)">
                    <span>{{ fileOfNode(v).originalName }}</span>
                    <span style="color:#909399;margin-left:6px;font-size:12px">({{ v.nodeId }})</span>
                  </template>
                  <span v-else style="color:#E6A23C">未上传</span>
                </template>
                <span v-else>{{ valueOfNode(v) }}</span>
              </el-descriptions-item>
            </el-descriptions>
          </template>
          <div v-else-if="detailData.variableValues" style="margin-top:14px">
            <div style="font-weight:600;margin-bottom:6px">输入内容</div>
            <pre class="detail-json">{{ formatJson(detailData.variableValues) }}</pre>
          </div>
          <template v-if="detailDialog.refFiles.length > 0">
            <el-divider content-position="left">参考文件</el-divider>
            <div class="ref-tags">
              <el-tag v-for="f in detailDialog.refFiles" :key="f.nodeId" size="small" :type="f.comfyType === 'video' ? 'warning' : 'primary'">
                {{ f.nodeId }}：{{ f.originalName }}
              </el-tag>
            </div>
          </template>
        </template>

        <!-- 输出 -->
        <template v-if="outputs(rowOfDetail).length > 0">
          <el-divider content-position="left">输出</el-divider>
          <div class="output-list">
            <template v-for="(o, i) in outputs(rowOfDetail).slice(0, 3)">
              <video v-if="o.type === 'video'" :key="'v' + i" :src="o.url" muted preload="metadata" class="output-thumb" @click="previewOutputs(rowOfDetail)" />
              <img v-else :key="'i' + i" :src="o.url" class="output-thumb" @click="previewOutputs(rowOfDetail)" />
            </template>
            <el-button v-if="outputs(rowOfDetail).length > 3" size="mini" type="text" @click="previewOutputs(rowOfDetail)">
              +{{ outputs(rowOfDetail).length - 3 }}
            </el-button>
          </div>
        </template>
      </div>

      <div slot="footer">
        <el-button size="mini" @click="detailVisible = false">关闭</el-button>
        <el-button v-if="detailDialog.mode === 'edit'" type="primary" size="mini" :loading="detailDialog.saving" @click="handleDetailSave">
          保存并入队
        </el-button>
      </div>
    </el-dialog>

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
import { getComfyuiTaskList, getComfyuiTaskDetail, getWorkflowVariables, updateComfyuiTask, translateComfyuiText, deleteComfyuiTask, batchDeleteComfyuiTask, enqueueComfyuiTask } from '@/api/comfyui/index'

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
      previewList: [],
      previewIndex: 0,
      detailVisible: false,
      detailData: null,
      detailDialog: {
        mode: 'view',
        loading: false,
        saving: false,
        row: null,
        variables: [],
        values: {},
        translateHint: {},
        refFiles: []
      },
      fileMap: {},
      fileListMap: {}
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
    previewOutputs(row) {
      const all = this.outputs(row)
      this.previewList = all.slice().sort((a, b) => {
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
    // ===== 任务明细 / 编辑 =====
    rowOfDetail() {
      return Object.assign({}, this.detailDialog.row, this.detailData || {})
    },
    isFileNode(v) { return v.type === 'image' || v.type === 'video' },
    parseJson(json, fallback) {
      if (!json) return fallback
      try { return JSON.parse(json) } catch (e) { return fallback }
    },
    fileOfNode(v) {
      if (!v) return null
      return (this.detailDialog.refFiles || []).find(f => f.nodeId === v.nodeId) || null
    },
    valueOfNode(v) {
      if (!v || v.type === 'image' || v.type === 'video') return '-'
      const values = this.parseJson(this.detailData ? this.detailData.variableValues : null, {})
      return values[v.nodeId] == null ? '-' : values[v.nodeId]
    },
    formatJson(json) {
      try { return JSON.stringify(JSON.parse(json), null, 2) } catch (e) { return json }
    },
    detailErrorText() {
      if (this.detailData && this.detailData.queueErrorMessage) return this.detailData.queueErrorMessage
      if (this.detailData) return this.detailData.errorMessage
      return ''
    },
    openDetail(row, mode) {
      if (mode === 'edit' && row.queueStatus === 'processing') {
        this.$message.warning('任务正在执行中，无法编辑，请稍候')
        return
      }
      this.detailVisible = true
      this.detailDialog.mode = mode
      this.detailDialog.loading = true
      this.detailDialog.saving = false
      this.detailDialog.row = row
      this.detailDialog.variables = []
      this.detailDialog.values = {}
      this.detailDialog.translateHint = {}
      this.detailDialog.refFiles = []
      this.fileMap = {}
      this.fileListMap = {}
      getComfyuiTaskDetail(row.id).then(res => {
        this.detailData = res.data
        this.detailDialog.refFiles = this.parseJson(res.data.refFiles, [])
        getWorkflowVariables(row.workflowId).then(r2 => {
          this.detailDialog.variables = (r2.data || []).filter(v => v.enabled !== false)
          if (mode === 'edit') {
            const values = this.parseJson(res.data.variableValues, {})
            this.detailDialog.variables.forEach(v => {
              this.$set(this.detailDialog.values, v.nodeId,
                values[v.nodeId] != null ? values[v.nodeId] : (v.type === 'bool' ? false : ''))
            })
          }
        }).finally(() => { this.detailDialog.loading = false })
      }).catch(() => { this.detailDialog.loading = false })
    },
    handleEditFileChange(nodeId, file) {
      this.$set(this.fileMap, nodeId, file.raw)
      this.$set(this.fileListMap, nodeId, [file])
    },
    handleEditFileRemove(nodeId) {
      this.$set(this.fileMap, nodeId, null)
      this.$set(this.fileListMap, nodeId, [])
    },
    handleSwitchLang(v) {
      const nodeId = v.nodeId
      const text = String(this.detailDialog.values[nodeId] == null ? '' : this.detailDialog.values[nodeId]).trim()
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
          this.$set(this.detailDialog.values, nodeId, translated)
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
        const text = String(this.detailDialog.values[nodeId] == null ? '' : this.detailDialog.values[nodeId]).trim()
        if (!text) {
          this.$set(this.detailDialog.translateHint, nodeId, '')
          return
        }
        // 已是中文则无需提示
        if (/[\u4e00-\u9fff]/.test(text)) {
          this.$set(this.detailDialog.translateHint, nodeId, '')
          return
        }
        translateComfyuiText({ text: text, target: 'zh-CN' }).then(res => {
          const t = res.data.translated
          this.$set(this.detailDialog.translateHint, nodeId, (t && t !== text) ? t : '')
        }).catch(() => {})
      }, 900)
    },
    handleDetailSave() {
      if (!this.detailData) return
      this.detailDialog.saving = true
      const fd = new FormData()
      fd.append('workflowId', this.detailData.workflowId)
      fd.append('funcType', this.detailData.funcType)
      const values = {}
      this.detailDialog.variables.forEach(v => {
        if (this.isFileNode(v)) return
        const val = String(this.detailDialog.values[v.nodeId] == null ? '' : this.detailDialog.values[v.nodeId]).trim()
        if (val) values[v.nodeId] = val
      })
      fd.append('variableValues', JSON.stringify(values))
      this.detailDialog.variables.forEach(v => {
        if (this.isFileNode(v) && this.fileMap[v.nodeId]) {
          fd.append('ref_' + v.nodeId, this.fileMap[v.nodeId])
        }
      })
      updateComfyuiTask(this.detailData.id, fd).then(res => {
        this.$message.success(res.data.message || '已更新')
        this.detailVisible = false
        this.getList()
      }).catch(err => {
        this.$message.error(err.msg || '保存失败')
      }).finally(() => { this.detailDialog.saving = false })
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
.upload-btn { display: inline-block; }
.detail-json {
  background: #f5f7fa;
  padding: 10px;
  border-radius: 4px;
  font-size: 12px;
  max-height: 200px;
  overflow-y: auto;
  white-space: pre-wrap;
}
.ref-tags { display: flex; flex-wrap: wrap; gap: 6px; }
.input-with-switch { display: flex; align-items: flex-start; gap: 8px; }
.input-with-switch .el-input { flex: 1; }
.lang-switch-btn { flex-shrink: 0; }
.translate-hint { margin-top: 4px; font-size: 12px; color: #606266; background: #f5f7fa; padding: 4px 8px; border-radius: 4px; line-height: 1.5; }
.hint-label { color: #409EFF; font-weight: 600; }
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
