<!--
 * @Descripttion: (/tb_result)
 * @version: (1.0)
 * @Author: (root)
 * @Date: (2025-08-25)
 * @LastEditors: (root)
 * @LastEditTime: (2025-08-25)
-->
<template>
  <div class="app-container">
    <el-row :gutter="12" class="mb8">
      <el-form :model="queryParams" size="small" label-position="right" inline ref="queryForm" label-width="100px"
        v-show="showSearch" @submit.native.prevent>
        <!--<el-col :span="6">
         <el-form-item label="开始日期" prop="操作开始时间">
            <el-date-picker v-model="queryParams.操作开始时间" type="date" placeholder="开始日期" value-format="yyyy-MM-dd"
              clearable>
            </el-date-picker>
          </el-form-item>
        </el-col>
        <el-col :span="6">
          <el-form-item label="结束日期" prop="操作结束时间">
            <el-date-picker v-model="queryParams.操作结束时间" type="date" placeholder="结束日期" value-format="yyyy-MM-dd"
              clearable>
            </el-date-picker>
          </el-form-item>
        </el-col>-->
        <el-col :span="6">
          <el-form-item label="商家名称" prop="商家名称">
            <el-input v-model="queryParams.商家名称" placeholder="请输入商家名称" clearable :style="{ width: '100%' }">
            </el-input>
          </el-form-item>
        </el-col>
        <el-col :span="6">
          <el-form-item label="收件人信息" prop="收件人信息">
            <el-input v-model="queryParams.收件人信息" placeholder="请输入收件人信息" clearable :style="{ width: '100%' }">
            </el-input>
          </el-form-item>
        </el-col>
        <!-- <el-col :span="6">
          <el-form-item label="运单号" prop="单号">
            <el-input v-model="queryParams.单号" placeholder="请输入运单号" clearable :style="{ width: '100%' }">
            </el-input>
          </el-form-item>
        </el-col> -->
        <!-- <el-col :span="6">
          <el-form-item label="处理状态" prop="处理状态">
            <el-select v-model="queryParams.处理状态" placeholder="请选择状态" clearable :style="{ width: '100%' }">
              <el-option v-for="(item, index) in 处理状态Options" :key="index" :label="item.label" :value="item.value"
                :disabled="item.disabled"></el-option>
            </el-select>
          </el-form-item>
        </el-col> -->

        <!-- <el-col :span="6">
          <el-form-item label="问题件类型" prop="问题件类型">
            <el-select v-model="queryParams.问题件类型" placeholder="请选择问题件类型" clearable :style="{ width: '100%' }">
              <el-option v-for="(item, index) in 问题件类型Options" :key="index" :label="item.label" :value="item.value"
                :disabled="item.disabled"></el-option>
            </el-select>
          </el-form-item>
        </el-col> -->
        <!-- <el-col :span="6">
          <el-form-item label="问题件类别" prop="问题件类别">
            <el-select v-model="queryParams.问题件类别" placeholder="请选择问题件类别" clearable :style="{ width: '100%' }">
              <el-option v-for="(item, index) in 问题件类别Options" :key="index" :label="item.label" :value="item.value"
                :disabled="item.disabled"></el-option>
            </el-select>
          </el-form-item>
        </el-col> -->
        <el-col :span="6">

          <el-button type="primary" icon="el-icon-search" size="mini" @click="handleQuery">搜索</el-button>
          <el-button icon="el-icon-refresh" size="mini" @click="resetQuery">重置</el-button>
        </el-col>

      </el-form>
    </el-row>

    <!-- 数据区域 -->
    <el-table :data="dataList" v-loading="loading" ref="table" border highlight-current-row>
      <!-- <el-table-column type="selection" width="50" align="center" /> -->
      <!-- <el-table-column prop="问题件类型" label="问题件类型" align="center" :show-overflow-tooltip="true" />
      <el-table-column prop="单号" label="单号" align="center" :show-overflow-tooltip="true" /> -->
      <el-table-column prop="商家名称" label="商家名称" align="center" :show-overflow-tooltip="true" />
      <el-table-column prop="收件人信息" label="收件人信息" align="center" :show-overflow-tooltip="true" />
      <el-table-column prop="count" label="数量" align="center" :show-overflow-tooltip="true" />
      <el-table-column prop="replyMessage" label="数量" align="center" :show-overflow-tooltip="true" />
      <!-- <el-table-column prop="执行机器人" label="执行机器人" align="center" :show-overflow-tooltip="true" />
      <el-table-column prop="操作时间" label="操作时间" align="center" :show-overflow-tooltip="true" /> -->
      <!-- <el-table-column prop="companyId" label="CompanyId" align="center" :show-overflow-tooltip="true" /> -->
      <el-table-column label="操作" align="center" width="140">
        <template slot-scope="scope">
          <el-button size="mini" type="success" icon="el-icon-edit" title="匹配客户群"
            @click="handleAdd(scope.row)"></el-button>
          <!-- <el-button size="mini" v-hasPermi="['tbresult:delete']" type="danger" icon="el-icon-delete" title="删除"
            @click="handleDelete(scope.row)"></el-button> -->
        </template>
      </el-table-column>
    </el-table>
    <pagination class="mt10" background :total="total" :page.sync="queryParams.pageNum"
      :limit.sync="queryParams.pageSize" @pagination="getList" />

    <!-- 添加或修改对话框 -->
    <el-dialog :title="title" :lock-scroll="false" :visible.sync="open">
      <TbContactFullComponent @rowClick="rowClickCallBack"></TbContactFullComponent>

    </el-dialog>



  </div>
</template>
<script>
import {
  listTbResultdistinctlist,
  addTbResult,
  delTbResult,
  updateTbResult,
  getTbResult,
  forwardMessage,
  copyMessage,
} from '@/api/business/tbResult.js';
import TbContactFullComponent from '@/views/business/TbContactFullComponent.vue';
import TbContactComponent2 from '@/views/business/TbContactComponent2.vue';
export default {
  name: "TbResultComponent",
  components: {
    TbContactFullComponent,
    TbContactComponent2
  },
  data() {
    return {
      selectIdModel: {
        name: '',
        phone: ''
      },
      labelWidth: "100px",
      formLabelWidth: "100px",
      // 选中id数组
      ids: [],
      // 非单个禁用
      single: true,
      // 非多个禁用
      multiple: true,
      // 遮罩层
      loading: false,
      // 显示搜索条件
      showSearch: true,
      // 查询参数
      queryParams: {
        操作开始时间: '',
        操作结束时间: '',
        商家名称: '',
        单号: '',
        处理状态: '',
        问题件类型: '',
        问题件类别: '',
        收件人信息: '',
        pageNum: 1,
        pageSize: 8,
        sort: undefined,
        sortType: undefined,
      },
      // 弹出层标题
      title: "",
      forwardTitle: "",
      // 操作类型 1、add 2、edit
      opertype: 0,
      // 是否显示弹出层
      open: false,
      forward: false,
      // 表单参数
      form: {},
      forwardForm: {},
      columns: [
        { index: 0, key: '问题件类型', label: `问题件类型`, checked: true },
        { index: 1, key: '单号', label: `单号`, checked: true },
        { index: 2, key: '商家名称', label: `商家名称`, checked: true },
        { index: 3, key: '收件人信息', label: `收件人信息`, checked: true },
        { index: 4, key: '结果', label: `结果`, checked: true },
        { index: 5, key: '执行机器人', label: `执行机器人`, checked: true },
        { index: 6, key: '操作时间', label: `操作时间`, checked: true },
        { index: 7, key: 'companyId', label: `CompanyId`, checked: true },
      ],
      dataList: [],
      total: 0,
      rules: {
      },
      处理状态Options: [{
        "label": "已处理",
        "value": "已处理"
      }, {
        "label": "未处理",
        "value": ""
      }
      ],
      问题件类型Options: [{
        "label": "拒收",
        "value": "拒收"
      }, {
        "label": "破损件",
        "value": "破损件"
      }, {
        "label": "信息有误",
        "value": "信息有误"
      }
      ],
      问题件类别Options: [{
        "label": "面单详情与实际内件不符",
        "value": "面单详情与实际内件不符"
      }, {
        "label": "地址错误",
        "value": "地址错误"
      }, {
        "label": "电话错误",
        "value": "电话错误"
      }, {
        "label": "空号",
        "value": "空号"
      }, {
        "label": "停机",
        "value": "停机"
      }, {
        "label": "双面单",
        "value": "双面单"
      }, {
        "label": "有单无货",
        "value": "有单无货"
      }
      ],
    };
  },
  created() {
    // 列表数据查询
    this.getList();

    var dictParams = [
    ];
  },
  methods: {
    rowClickCallBack(row) {
      console.log(row);
    },
    // 清空选中状态
    clearAllCheck() {
      this.$refs.table.clearSelection();
      this.ids = [];
      this.single = true;
      this.multiple = true;
    },
    handleSelectAll(selection) {
      console.log(selection);

    },
    // 转发商户
    handleForward() {
      let page = this;
      if (page.ids.length == 0) {
        page.$message({
          message: '警告，请至少选择一条数据进行操作',
          type: 'warning'
        });
        return;
      }

      page.$confirm('是否确认转发选中的数据？')
        .then(function () {

          page.forward = true;

          forwardMessage(page.ids.toString()).then(res => {
            if (res.code == 200) {
              page.forwardTitle = `${res.data.bussinessName} ${res.data.sendUser}`;
              page.forwardForm = res.data;
            }
          })


          // this.msgSuccess("转发成功");
        })
        .catch((e) => { console.log(e) });
    },
    // 查询数据
    getList() {
      this.loading = true;
      listTbResultdistinctlist(this.queryParams).then(res => {
        if (res.code == 200) {
          this.dataList = res.data.result;
          this.total = res.data.totalNum;
          this.loading = false;
        }
      })
    },
    // 取消按钮
    cancel() {
      this.open = false;
      this.reset();
    },
    // 取消按钮
    forwardCancel() {
      this.forward = false;
    },
    // 重置数据表单
    reset() {

      this.form = {
        操作开始时间: '',
        操作结束时间: '',
        商家名称: '',
        单号: '',
        处理状态: '',
        问题件类型: '',
        问题件类别: '',
        收件人信息: '',
        结果: '',
        执行机器人: '',
        companyId: '',
      };
      this.resetForm("form");
    },
    // 重置查询操作
    resetQuery() {

      this.resetForm("queryForm");
      this.handleQuery();
    },
    // 表格选中时
    handleSelectionChange(selection) {
      this.ids = selection.map((item) => item.id);
      this.single = selection.length != 1
      this.multiple = !selection.length;
      if (selection.length > 0) {
        let name = selection[0].商家名称;
        let phone = selection[0].收件人信息;
        this.queryParams.商家名称 = name;
        this.queryParams.收件人信息 = phone;
        let needClearSelectDataList = this.dataList.filter(item => item.商家名称 != name || item.收件人信息 != phone);
        needClearSelectDataList.forEach((item) => {
          this.$refs.table.toggleRowSelection(item, false);
        });
      }
    },
    // 自定义排序
    sortChange(column) {
      if (column.prop == null || column.order == null) {
        this.queryParams.sort = undefined;
        this.queryParams.sortType = undefined;
      } else {
        this.queryParams.sort = column.prop;
        this.queryParams.sortType = column.order;
      }

      this.handleQuery();
    },
    /** 搜索按钮操作 */
    handleQuery() {
      this.queryParams.pageNum = 1;
      this.getList();
    },
    /** 新增按钮操作 */
    handleAdd() {
      this.reset();
      this.open = true;
      this.title = "添加";
      this.opertype = 1;
    },
    /** 删除按钮操作 */
    handleDelete(row) {
      const Ids = row.id || this.ids;

      this.$confirm('是否确认删除参数编号为"' + Ids + '"的数据项？')
        .then(function () {
          return delTbResult(Ids);
        })
        .then(() => {
          this.handleQuery();
          this.msgSuccess("删除成功");
        })
        .catch(() => { });
    },
    /** 修改按钮操作 */
    handleUpdate(row) {
      this.reset();
      const id = row.id || this.ids;
      getTbResult(id).then((res) => {
        const { code, data } = res;
        if (code == 200) {
          this.open = true;
          this.title = "修改数据";
          this.opertype = 2;

          this.form = {
            ...data,
          };
        }
      });
    },
    /** 提交按钮 */
    submitForm: function () {
      this.$refs["form"].validate((valid) => {
        if (valid) {
          if (this.form.id != undefined && this.opertype === 2) {
            updateTbResult(this.form)
              .then((res) => {
                this.msgSuccess("修改成功");
                this.open = false;
                this.getList();
              })
          } else {
            addTbResult(this.form)
              .then((res) => {
                this.msgSuccess("新增成功");
                this.open = false;
                this.getList();
              })
          }
        }
      });
    },
  },
};
</script>
