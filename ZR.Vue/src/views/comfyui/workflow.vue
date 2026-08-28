<template>
  <div class="app-container">
    <el-card class="box-card">
      <div slot="header" class="clearfix">
        <span>ComfyUI 工作流管理</span>
      </div>

      <el-form :model="queryParams" ref="queryForm" :inline="true" label-width="68px">
        <el-form-item label="名称" prop="name">
          <el-input v-model="queryParams.name" placeholder="模糊搜索名称" clearable size="small" @keyup.enter.native="handleQuery" />
        </el-form-item>
        <el-form-item label="分类" prop="category">
          <el-select v-model="queryParams.category" placeholder="全部分类" clearable size="small">
            <el-option label="默认" value="default" />
            <el-option label="文生图" value="txt2img" />
            <el-option label="图生图" value="img2img" />
            <el-option label="文生视频" value="txt2video" />
            <el-option label="图生视频" value="img2video" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" icon="el-icon-search" size="mini" @click="handleQuery">搜索</el-button>
          <el-button icon="el-icon-refresh" size="mini" @click="resetQuery">重置</el-button>
        </el-form-item>
      </el-form>

      <el-row :gutter="10" class="mb8">
        <el-col :span="1.5">
          <el-button type="primary" plain icon="el-icon-upload2" size="mini" @click="handleImport">导入工作流</el-button>
        </el-col>
        <el-col :span="1.5">
          <el-button type="warning" plain icon="el-icon-setting" size="mini" :disabled="multiple" @click="handleSetCategory">设置分类</el-button>
        </el-col>
        <el-col :span="1.5">
          <el-button type="danger" plain icon="el-icon-delete" size="mini" :disabled="multiple" @click="handleBatchDelete">批量删除</el-button>
        </el-col>
      </el-row>

      <el-table ref="workflowList" v-loading="loading" :data="workflowList" border stripe @selection-change="handleSelectionChange">
        <el-table-column type="selection" width="55" align="center" />
        <el-table-column label="名称" prop="name" min-width="150" :show-overflow-tooltip="true" />
        <el-table-column label="分类" prop="category" width="100" align="center">
          <template slot-scope="scope">
            <el-tag size="small" :type="categoryTagType(scope.row.category)">{{ categoryText[scope.row.category] || scope.row.category }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column label="节点数" prop="nodeCount" width="80" align="center" />
        <el-table-column label="描述" prop="description" min-width="200" :show-overflow-tooltip="true" />
        <el-table-column label="创建时间" prop="createTime" width="160" align="center" />
        <el-table-column label="操作" width="180" align="center" fixed="right">
          <template slot-scope="scope">
            <el-button type="text" icon="el-icon-view" @click="handleView(scope.row)">查看</el-button>
            <el-button type="text" icon="el-icon-edit-outline" @click="handleEditVariables(scope.row)">可变节点</el-button>
            <el-button type="text" icon="el-icon-delete" style="color:#F56C6C" @click="handleDelete(scope.row)">删除</el-button>
          </template>
        </el-table-column>
      </el-table>

      <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum" :limit.sync="queryParams.pageSize" @pagination="getList" />
    </el-card>

    <!-- 导入工作流弹窗 -->
    <el-dialog title="导入工作流" :visible.sync="importDialogVisible" width="700px" top="5vh">
      <el-tabs v-model="importMode" type="border-card">
        <!-- 粘贴JSON -->
        <el-tab-pane label="粘贴JSON" name="paste">
          <el-form :model="importForm" label-width="80px">
            <el-form-item label="工作流JSON" required>
              <el-input v-model="importForm.workflowJson" type="textarea" :rows="10" placeholder="仅支持ComfyUI API格式。请在ComfyUI中：菜单→Workflow→Export (API) 导出JSON后粘贴到这里" />
              <div style="color:#E6A23C;font-size:12px;line-height:1.6;margin-top:4px">
                注意：只接受【API格式】（顶层是节点对象，每个节点含 class_type 和 inputs）。
                若从 ComfyUI 直接保存/复制看到的带 nodes 数组的 UI 格式，请用「Workflow → Export (API)」重新导出。
              </div>
            </el-form-item>
            <el-form-item label="名称" required>
              <el-input v-model="importForm.name" placeholder="给工作流起个名字" />
            </el-form-item>
            <el-form-item label="分类">
              <el-select v-model="importForm.category" placeholder="选择分类">
                <el-option label="默认" value="default" />
                <el-option label="文生图" value="txt2img" />
                <el-option label="图生图" value="img2img" />
                <el-option label="文生视频" value="txt2video" />
                <el-option label="图生视频" value="img2video" />
              </el-select>
            </el-form-item>
            <el-form-item label="描述">
              <el-input v-model="importForm.description" placeholder="工作流描述（可选）" />
            </el-form-item>
            <el-form-item label="标签">
              <el-input v-model="importForm.tags" placeholder="标签（逗号分隔，可选）" />
            </el-form-item>
            <el-form-item label="可变节点">
              <el-input v-model="importForm.variableNodes" type="textarea" :rows="4"
                placeholder='可选。数组JSON，定义需要用户填写的节点，如：[{"nodeId":"6","field":"text","type":"prompt","label":"正向提示词"}]。type可选 prompt/image/video/value/number/bool' />
            </el-form-item>
          </el-form>
        </el-tab-pane>

        <!-- 上传JSON文件 -->
        <el-tab-pane label="上传文件" name="file">
          <el-upload
            ref="jsonUpload"
            class="json-uploader"
            drag
            action="#"
            :auto-upload="false"
            :on-change="handleJsonFileChange"
            :on-remove="handleJsonFileRemove"
            :file-list="jsonFileList"
            accept=".json"
            multiple
          >
            <i class="el-icon-upload"></i>
            <div class="el-upload__text">将JSON文件拖到此处，或<em>点击上传</em></div>
            <div slot="tip" class="el-upload__tip">支持上传多个ComfyUI API格式的JSON文件，文件名将作为工作流名称</div>
          </el-upload>

          <el-form v-if="jsonFileList.length > 0" label-width="80px" style="margin-top: 16px">
            <el-form-item label="统一分类">
              <el-select v-model="batchCategory" placeholder="选择分类">
                <el-option label="默认" value="default" />
                <el-option label="文生图" value="txt2img" />
                <el-option label="图生图" value="img2img" />
                <el-option label="文生视频" value="txt2video" />
                <el-option label="图生视频" value="img2video" />
              </el-select>
            </el-form-item>
            <el-form-item label="统一标签">
              <el-input v-model="batchTags" placeholder="标签（逗号分隔，可选）" />
            </el-form-item>
            <el-form-item>
              <span style="color: #909399; font-size: 12px">已选择 {{ jsonFileList.length }} 个文件</span>
            </el-form-item>
          </el-form>
        </el-tab-pane>
      </el-tabs>

      <span slot="footer">
        <el-button @click="importDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="confirmImport" :loading="importLoading">导入</el-button>
      </span>
    </el-dialog>

    <!-- 查看工作流弹窗 -->
    <el-dialog title="工作流详情" :visible.sync="viewDialogVisible" width="800px" top="5vh">
      <div v-if="viewWorkflow">
        <el-descriptions :column="2" border>
          <el-descriptions-item label="名称">{{ viewWorkflow.name }}</el-descriptions-item>
          <el-descriptions-item label="分类">
            <el-tag size="small" :type="categoryTagType(viewWorkflow.category)">{{ categoryText[viewWorkflow.category] || viewWorkflow.category }}</el-tag>
          </el-descriptions-item>
          <el-descriptions-item label="节点数">{{ viewWorkflow.nodeCount }}</el-descriptions-item>
          <el-descriptions-item label="创建时间">{{ viewWorkflow.createTime }}</el-descriptions-item>
          <el-descriptions-item label="描述" :span="2">{{ viewWorkflow.description || '-' }}</el-descriptions-item>
        </el-descriptions>
        <div style="margin-top: 16px">
          <div style="font-weight: bold; margin-bottom: 8px">工作流JSON</div>
          <el-input v-model="viewWorkflow.workflowJson" type="textarea" :rows="15" readonly />
        </div>
      </div>
    </el-dialog>

    <!-- 设置分类弹窗 -->
    <el-dialog title="设置分类" :visible.sync="categoryDialogVisible" width="400px">
      <el-form label-width="80px">
        <el-form-item label="分类">
          <el-select v-model="selectedCategory" placeholder="选择分类" style="width:100%">
            <el-option label="默认" value="default" />
            <el-option label="文生图" value="txt2img" />
            <el-option label="图生图" value="img2img" />
            <el-option label="文生视频" value="txt2video" />
            <el-option label="图生视频" value="img2video" />
          </el-select>
        </el-form-item>
      </el-form>
      <span slot="footer">
        <el-button @click="categoryDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="confirmSetCategory" :loading="categoryLoading">确定</el-button>
      </span>
    </el-dialog>

    <!-- 可变节点配置弹窗 -->
    <el-dialog title="配置可变节点" :visible.sync="varDialogVisible" width="960px" top="4vh" :close-on-click-modal="false">
      <div style="margin-bottom: 8px; color: #909399; font-size: 12px">
        系统已自动筛查出工作流中【可修改的节点】并附上用途说明，只需从中勾选需要用户填写的节点，设置类型后保存。保存后文生图/图生图等表单会按类型自动生成对应输入元素。
      </div>
      <div v-if="editableNodeLoading" style="text-align:center;padding:20px"><i class="el-icon-loading" style="font-size:24px"></i> 正在解析可修改节点...</div>
      <el-alert v-if="!editableNodeLoading && editableNodes.length === 0" type="warning" :closable="false" style="margin-bottom:8px"
        title="未自动解析出可修改节点（不影响手动添加）。请确认工作流为ComfyUI API格式；你也可以直接点下方【添加节点】手动填写节点ID与字段。" show-icon />
      <el-table v-if="!editableNodeLoading" :data="varRows" border size="small">
        <el-table-column label="启用" width="56" align="center">
          <template slot-scope="scope">
            <el-switch v-model="scope.row.enabled" />
          </template>
        </el-table-column>
        <el-table-column label="可修改节点" min-width="180">
          <template slot-scope="scope">
            <el-select v-model="scope.row.nodeId" filterable allow-create default-first-option size="small" style="width:100%"
              :disabled="!scope.row.enabled" placeholder="从可修改节点选择或手动输入ID" @change="(v) => onSelectEditableNode(scope.row, v)">
              <el-option v-for="n in editableNodes" :key="n.nodeId" :value="n.nodeId"
                :label="(n.description || n.title) + ' (#' + n.nodeId + ')'"
                :disabled="isNodeUsed(n.nodeId, scope.row)">
                <div style="line-height:1.3">
                  <div><b>{{ n.title }}</b>&nbsp;<span style="color:#909399;font-size:11px">{{ n.classType }}</span></div>
                  <div style="color:#67C23A;font-size:11px">{{ n.description }}</div>
                  <div v-if="n.fields && n.fields.length" style="color:#409EFF;font-size:11px">可改字段: {{ n.fields.map(f => f.field).join('、') }}</div>
                </div>
              </el-option>
            </el-select>
          </template>
        </el-table-column>
        <el-table-column label="修改字段" width="130">
          <template slot-scope="scope">
            <el-select v-model="scope.row.field" filterable allow-create default-first-option size="small" style="width:100%" :disabled="!scope.row.enabled || !scope.row.nodeId" placeholder="选择或输入字段名">
              <el-option v-for="f in fieldOptionsFor(scope.row)" :key="f.field" :label="`${f.field} (${f.description})`" :value="f.field" />
              <el-option label="(不需要字段)" value="" />
            </el-select>
          </template>
        </el-table-column>
        <el-table-column label="类型" width="110" align="center">
          <template slot-scope="scope">
            <el-select v-model="scope.row.type" size="small" style="width:100%" :disabled="!scope.row.enabled">
              <el-option label="文字" value="prompt" />
              <el-option label="开关" value="bool" />
              <el-option label="图片" value="image" />
              <el-option label="视频" value="video" />
              <el-option label="数值" value="value" />
            </el-select>
          </template>
        </el-table-column>
        <el-table-column label="显示名称" min-width="200">
          <template slot-scope="scope">
            <el-input v-model="scope.row.label" size="small" placeholder="表单显示名称" :disabled="!scope.row.enabled" />
          </template>
        </el-table-column>
        <el-table-column label="操作" width="64" align="center">
          <template slot-scope="scope">
            <el-button type="text" size="small" icon="el-icon-delete" style="color:#F56C6C" @click="removeVarRow(scope.$index)">删</el-button>
          </template>
        </el-table-column>
      </el-table>
      <div style="margin-top: 12px; text-align: right">
        <el-button size="small" icon="el-icon-plus" @click="addVarRow">添加节点</el-button>
      </div>
      <span slot="footer">
        <el-button @click="varDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="confirmVariables" :loading="varLoading">保存</el-button>
      </span>
    </el-dialog>
  </div>
</template>

<script>
import { importWorkflows, getWorkflowList, getWorkflowDetail, setWorkflowCategory, deleteWorkflow, updateWorkflowVariables, getEditableNodes } from '@/api/comfyui/index'

export default {
  name: 'ComfyuiWorkflow',
  data() {
    return {
      workflowList: [],
      loading: false,
      total: 0,
      queryParams: { pageNum: 1, pageSize: 20, name: '', category: '' },
      categoryText: { default: '默认', txt2img: '文生图', img2img: '图生图', txt2video: '文生视频', img2video: '图生视频' },
      ids: [],
      multiple: true,
      // 导入
      importDialogVisible: false,
      importLoading: false,
      importMode: 'paste',
      importForm: { name: '', description: '', category: 'default', workflowJson: '', tags: '' },
      jsonFileList: [],
      batchCategory: 'default',
      batchTags: '',
      // 查看
      viewDialogVisible: false,
      viewWorkflow: null,
      // 分类
      categoryDialogVisible: false,
      categoryLoading: false,
      selectedCategory: 'default',
      // 可变节点
      varDialogVisible: false,
      varLoading: false,
      editableNodeLoading: false,
      varForm: { workflowId: null, workflowJson: '' },
      varRows: [],
      editableNodes: []
    }
  },
  created() { this.getList() },
  methods: {
    getList() {
      this.loading = true
      getWorkflowList(this.queryParams).then(res => {
        this.workflowList = res.data.result
        this.total = res.data.totalNum
      }).finally(() => { this.loading = false })
    },
    handleQuery() { this.queryParams.pageNum = 1; this.getList() },
    resetQuery() { this.queryParams = { pageNum: 1, pageSize: 20, name: '', category: '' }; this.getList() },
    handleSelectionChange(selection) {
      this.ids = selection.map(item => item.id)
      this.multiple = !selection.length
    },
    handleImport() {
      this.importForm = { name: '', description: '', category: 'default', workflowJson: '', tags: '', variableNodes: '' }
      this.jsonFileList = []
      this.batchCategory = 'default'
      this.batchTags = ''
      this.importMode = 'paste'
      this.importDialogVisible = true
    },
    handleJsonFileChange(file, fileList) {
      this.jsonFileList = fileList
    },
    handleJsonFileRemove(file, fileList) {
      this.jsonFileList = fileList
    },
    async confirmImport() {
      if (this.importMode === 'paste') {
        if (!this.importForm.name) { this.$message.warning('请输入工作流名称'); return }
        if (!this.importForm.workflowJson) { this.$message.warning('请输入工作流JSON'); return }
        const err = this.checkApiFormat(this.importForm.workflowJson)
        if (err) { this.$message.error(err); return }
        this.importLoading = true
        importWorkflows({ workflows: [this.importForm] }).then(() => {
          this.$message.success('导入成功')
          this.importDialogVisible = false
          this.getList()
        }).catch(e => { this.$message.error((e && e.msg) || '导入失败，请确认JSON为API格式') }).finally(() => { this.importLoading = false })
      } else {
        if (this.jsonFileList.length === 0) { this.$message.warning('请选择JSON文件'); return }
        this.importLoading = true
        try {
          const workflows = []
          for (const item of this.jsonFileList) {
            const content = await this.readFileContent(item.raw)
            const err = this.checkApiFormat(content)
            if (err) { throw new Error(`${item.name} 不符合要求：${err}`) }
            const name = item.name.replace(/\.json$/i, '')
            workflows.push({
              name: name,
              workflowJson: content,
              category: this.batchCategory || 'default',
              tags: this.batchTags || ''
            })
          }
          await importWorkflows({ workflows })
          this.$message.success(`成功导入 ${workflows.length} 个工作流`)
          this.importDialogVisible = false
          this.getList()
        } catch (e) {
          this.$message.error('导入失败: ' + (e.message || '未知错误'))
        } finally {
          this.importLoading = false
        }
      }
    },
    checkApiFormat(json) {
      if (!json) return 'JSON不能为空'
      let obj = null
      try { obj = JSON.parse(json) } catch (e) { return 'JSON格式无效，无法解析' }
      if (obj && Array.isArray(obj.nodes)) {
        return '这是ComfyUI UI/编辑器格式（含nodes数组），仅支持API格式。请在ComfyUI用「Workflow → Export (API)」重新导出。'
      }
      if (typeof obj === 'string') {
        return 'JSON是字符串形式（被二次编码），请粘贴纯API格式对象。'
      }
      if (obj === null || typeof obj !== 'object') {
        return 'JSON顶层必须是对象（节点集合）。'
      }
      const keys = Object.keys(obj)
      if (keys.length === 0) return 'JSON为空，未包含任何节点'
      for (const k of keys) {
        const node = obj[k]
        if (!node || typeof node !== 'object' || Array.isArray(node)) {
          return `节点 [${k}] 不是对象，不符合API格式`
        }
        if (!node.class_type) {
          return `节点 [${k}] 缺少 class_type，不符合API格式。请用「Export (API)」导出。`
        }
      }
      return null
    },
    readFileContent(file) {
      return new Promise((resolve, reject) => {
        const reader = new FileReader()
        reader.onload = (e) => resolve(e.target.result)
        reader.onerror = (e) => reject(e)
        reader.readAsText(file)
      })
    },
    handleView(row) {
      getWorkflowDetail(row.id).then(res => {
        this.viewWorkflow = res.data
        this.viewDialogVisible = true
      })
    },
    handleEditVariables(row) {
      this.varForm.workflowId = row.id
      this.varRows = []
      this.editableNodes = []
      this.editableNodeLoading = true
      this.varDialogVisible = true
      // 筛查可编辑节点（带描述）
      getEditableNodes(row.id).then(res => {
        this.editableNodes = Array.isArray(res.data) ? res.data : []
        return getWorkflowDetail(row.id)
      }).then(r => {
        const wf = r.data
        this.varForm.workflowJson = (wf && wf.workflowJson) ? wf.workflowJson : ''
        const existing = this.parseVariableNodes(wf && wf.variableNodes)
        existing.forEach(n => {
          this.varRows.push({ enabled: true, nodeId: n.nodeId, type: n.type, field: n.field || '', label: n.label || n.nodeId })
        })
        if (this.varRows.length === 0) this.varRows.push(this.emptyVarRow())
      }).catch(err => {
        console.error('加载可变节点失败', err)
        this.editableNodes = []
      }).finally(() => {
        this.editableNodeLoading = false
      })
    },
    parseVariableNodes(str) {
      if (!str) return []
      try { const arr = JSON.parse(str); return Array.isArray(arr) ? arr : [] } catch (e) { return [] }
    },
    fieldOptionsFor(row) {
      const node = this.editableNodes.find(n => n.nodeId === row.nodeId)
      return node ? (node.fields || []) : []
    },
    onSelectEditableNode(row, nodeId) {
      const node = this.editableNodes.find(n => n.nodeId === nodeId)
      if (!node) return
      row.field = (node.fields && node.fields.length) ? node.fields[0].field : ''
      row.type = node.type || 'prompt'
      row.label = node.title ? node.title.replace(/\s*\(#\d+\)\s*$/, '') : nodeId
    },
    isNodeUsed(nodeId, currentRow) {
      return this.varRows.some(r => r !== currentRow && r.enabled && r.nodeId === nodeId)
    },
    emptyVarRow() {
      return { enabled: true, nodeId: '', type: 'prompt', field: '', label: '' }
    },
    addVarRow() {
      this.varRows.push(this.emptyVarRow())
    },
    removeVarRow(index) {
      this.varRows.splice(index, 1)
      if (this.varRows.length === 0) this.varRows.push(this.emptyVarRow())
    },
    confirmVariables() {
      const rows = this.varRows.filter(r => r.enabled && r.nodeId)
      if (rows.length === 0) {
        this.$message.warning('请至少启用一个节点')
        return
      }
      for (const r of rows) {
        if (!r.label) { this.$message.warning(`节点 ${r.nodeId} 请填写显示名称`); return }
      }
      const value = JSON.stringify(rows.map(r => ({
        nodeId: r.nodeId,
        field: r.field || 'text',
        type: r.type || 'prompt',
        label: r.label || r.nodeId
      })))
      this.varLoading = true
      updateWorkflowVariables(this.varForm.workflowId, value).then(res => {
        this.$message.success(res.data.message || '保存成功')
        this.varDialogVisible = false
        this.getList()
      }).catch(() => { this.$message.error('保存失败') }).finally(() => { this.varLoading = false })
    },
    handleDelete(row) {
      this.$confirm('确定删除该工作流?', '警告', { type: 'warning' }).then(() => {
        deleteWorkflow(row.id).then(() => { this.$message.success('已删除'); this.getList() })
      }).catch(() => {})
    },
    handleSetCategory() {
      if (this.ids.length === 0) { this.$message.warning('请先选择工作流'); return }
      this.selectedCategory = 'default'
      this.categoryDialogVisible = true
    },
    confirmSetCategory() {
      this.categoryLoading = true
      setWorkflowCategory({ ids: this.ids, category: this.selectedCategory }).then(res => {
        this.$message.success(res.data.message)
        this.categoryDialogVisible = false
        this.getList()
      }).catch(() => { this.$message.error('操作失败') }).finally(() => { this.categoryLoading = false })
    },
    handleBatchDelete() {
      if (this.ids.length === 0) { this.$message.warning('请先选择工作流'); return }
      this.$confirm(`确定删除选中的 ${this.ids.length} 个工作流？`, '警告', { type: 'warning' }).then(() => {
        let deleted = 0
        const total = this.ids.length
        const deleteNext = (index) => {
          if (index >= this.ids.length) {
            this.$message.success(`成功删除 ${deleted} 个工作流`)
            this.getList()
            return
          }
          deleteWorkflow(this.ids[index]).then(() => { deleted++; deleteNext(index + 1) }).catch(() => { deleteNext(index + 1) })
        }
        deleteNext(0)
      }).catch(() => {})
    },
    categoryTagType(cat) {
      const map = { default: 'info', txt2img: '', img2img: 'success', txt2video: 'warning', img2video: 'danger' }
      return map[cat] || 'info'
    }
  }
}
</script>

<style scoped>
.mb8 { margin-bottom: 12px; }
.json-uploader { width: 100%; }
.json-uploader .el-upload-dragger { width: 100%; }
</style>
