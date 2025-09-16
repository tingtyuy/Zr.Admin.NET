<!--
 * @Descripttion: (/company)
 * @version: (1.0)
 * @Author: (root)
 * @Date: (2025-09-16)
 * @LastEditors: (root)
 * @LastEditTime: (2025-09-16)
-->
<template>
  <div class="app-container">
    <el-form :model="queryParams" size="small" label-position="right" inline ref="queryForm" label-width="100px"
      v-show="showSearch" @submit.native.prevent>

      <el-form-item>
        <el-button type="primary" icon="el-icon-search" size="mini" @click="handleQuery">搜索</el-button>
        <el-button icon="el-icon-refresh" size="mini" @click="resetQuery">重置</el-button>
      </el-form-item>
    </el-form>
    <el-row :gutter="10" class="mb8">
      <el-col :span="1.5">
        <el-button type="primary" v-hasPermi="['company:add']" plain icon="el-icon-plus" size="mini"
          @click="handleAdd">新增</el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button type="success" :disabled="single" v-hasPermi="['company:edit']" plain icon="el-icon-edit" size="mini"
          @click="handleUpdate">修改</el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button type="danger" :disabled="multiple" v-hasPermi="['company:delete']" plain icon="el-icon-delete"
          size="mini" @click="handleDelete">删除</el-button>
      </el-col>
      <right-toolbar :showSearch.sync="showSearch" @queryTable="getList"></right-toolbar>
    </el-row>

    <!-- 数据区域 -->
    <el-table :data="dataList" v-loading="loading" ref="table" border highlight-current-row @sort-change="sortChange"
      @selection-change="handleSelectionChange">
      <el-table-column type="selection" width="50" align="center" />
      <el-table-column prop="id" label="主键，自增1" align="center" />
      <el-table-column prop="companyId" label="CompanyId" align="center" :show-overflow-tooltip="true" />
      <el-table-column prop="companyName" label="CompanyName" align="center" :show-overflow-tooltip="true" />
      <el-table-column prop="createtime" label="Createtime" align="center" :show-overflow-tooltip="true" />
      <el-table-column prop="state" label="1：使用中2：禁用3：测试中" align="center">
        <template slot-scope="scope">
          <dict-tag :options="stateOptions" :value="scope.row.state" />
        </template>
      </el-table-column>
      <el-table-column prop="isfixedStaff" label="1：是 2：否" align="center">
        <template slot-scope="scope">
          <dict-tag :options="isfixedStaffOptions" :value="scope.row.isfixedStaff" />
        </template>
      </el-table-column>
      <el-table-column prop="staffName" label="如果不填，就不筛选" align="center" :show-overflow-tooltip="true" />
      <el-table-column prop="emailTo" label="异常邮件通知" align="center" :show-overflow-tooltip="true" />
      <el-table-column prop="emailCC" label="EmailCC" align="center" :show-overflow-tooltip="true" />

      <el-table-column label="操作" align="center" width="140">
        <template slot-scope="scope">
          <el-button size="mini" v-hasPermi="['company:edit']" type="success" icon="el-icon-edit" title="编辑"
            @click="handleUpdate(scope.row)"></el-button>
          <el-button size="mini" v-hasPermi="['company:delete']" type="danger" icon="el-icon-delete" title="删除"
            @click="handleDelete(scope.row)"></el-button>
        </template>
      </el-table-column>
    </el-table>
    <pagination class="mt10" background :total="total" :page.sync="queryParams.pageNum"
      :limit.sync="queryParams.pageSize" @pagination="getList" />

    <!-- 添加或修改对话框 -->
    <el-dialog :title="title" :lock-scroll="false" :visible.sync="open">
      <el-form ref="form" :model="form" :rules="rules" label-width="100px">
        <el-row :gutter="20">

          <el-col :lg="12">
            <el-form-item label="主键，自增1" prop="id">
              <el-input-number v-model.number="form.id" controls-position="right" placeholder="请输入主键，自增1"
                :disabled="title == '修改数据'" />
            </el-form-item>
          </el-col>

          <el-col :lg="12">
            <el-form-item label="CompanyId" prop="companyId">
              <el-input v-model="form.companyId" placeholder="请输入CompanyId" />
            </el-form-item>
          </el-col>

          <el-col :lg="12">
            <el-form-item label="CompanyName" prop="companyName">
              <el-input v-model="form.companyName" placeholder="请输入CompanyName" />
            </el-form-item>
          </el-col>


          <el-col :lg="12">
            <el-form-item label="1：使用中
2：禁用
3：测试中" prop="state">
              <el-radio-group v-model="form.state">
                <el-radio v-for="item in stateOptions" :key="item.dictValue" :label="parseInt(item.dictValue)">{{
                  item.dictLabel }}</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>

          <el-col :lg="12">
            <el-form-item label="1：是 2：否" prop="isfixedStaff">
              <el-radio-group v-model="form.isfixedStaff">
                <el-radio v-for="item in isfixedStaffOptions" :key="item.dictValue" :label="parseInt(item.dictValue)">{{
                  item.dictLabel }}</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>

          <el-col :lg="12">
            <el-form-item label="如果不填，就不筛选" prop="staffName">
              <el-input v-model="form.staffName" placeholder="请输入如果不填，就不筛选" />
            </el-form-item>
          </el-col>

          <el-col :lg="12">
            <el-form-item label="异常邮件通知" prop="emailTo">
              <el-input v-model="form.emailTo" placeholder="请输入异常邮件通知" />
            </el-form-item>
          </el-col>

          <el-col :lg="12">
            <el-form-item label="EmailCC" prop="emailCC">
              <el-input v-model="form.emailCC" placeholder="请输入EmailCC" />
            </el-form-item>
          </el-col>

        </el-row>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button type="text" @click="cancel">取 消</el-button>
        <el-button type="primary" @click="submitForm">确 定</el-button>
      </div>
    </el-dialog>

  </div>
</template>
<script>
import {
  listCompany,
  addCompany,
  delCompany,
  updateCompany,
  getCompany,
} from '@/api/business/company.js';

export default {
  name: "company",
  data() {
    return {
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
        pageNum: 1,
        pageSize: 10,
        sort: undefined,
        sortType: undefined,
      },
      // 弹出层标题
      title: "",
      // 操作类型 1、add 2、edit
      opertype: 0,
      // 是否显示弹出层
      open: false,
      // 表单参数
      form: {},
      columns: [
        { index: 0, key: 'id', label: `主键，自增1`, checked: true },
        { index: 1, key: 'companyId', label: `CompanyId`, checked: true },
        { index: 2, key: 'companyName', label: `CompanyName`, checked: true },
        { index: 3, key: 'createtime', label: `Createtime`, checked: true },
        {
          index: 4, key: 'state', label: `1：使用中
2：禁用
3：测试中`, checked: true
        },
        { index: 5, key: 'isfixedStaff', label: `1：是 2：否`, checked: true },
        { index: 6, key: 'staffName', label: `如果不填，就不筛选`, checked: true },
        { index: 7, key: 'emailTo', label: `异常邮件通知`, checked: true },
        { index: 8, key: 'emailCC', label: `EmailCC`, checked: true },
      ],
      // 1：使用中
      ////2：禁用
      //3：测试中选项列表 格式 eg: { dictLabel: '标签', dictValue: '0' }
      stateOptions: [],
      // 1：是 2：否选项列表 格式 eg:{ dictLabel: '标签', dictValue: '0'}
      isfixedStaffOptions: [],
      dataList: [],
      total: 0,
      rules: {
        id: [
          { required: true, message: "主键，自增1不能为空", trigger: "blur" }
        ],
        companyId: [
          { required: true, message: "CompanyId不能为空", trigger: "blur" }
        ],
        companyName: [
          { required: true, message: "CompanyName不能为空", trigger: "blur" }
        ],
        createtime: [
          { required: true, message: "Createtime不能为空", trigger: "blur" }
        ],
        state: [
          {
            required: true, message: "1：使用中2：禁用3：测试中不能为空", trigger: "blur" }
        ],
        isfixedStaff: [
          { required: true, message: "1：是 2：否不能为空", trigger: "blur" }
        ],
        emailTo: [
          { required: true, message: "异常邮件通知不能为空", trigger: "blur" }
        ],
      },
    };
  },
  created() {
    // 列表数据查询
    this.getList();

    var dictParams = [
    ];
  },
  methods: {
    // 查询数据
    getList() {
      this.loading = true;
      listCompany(this.queryParams).then(res => {
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
    // 重置数据表单
    reset() {
      this.form = {
        id: undefined,
        companyId: undefined,
        companyName: undefined,
        createtime: undefined,
        state: undefined,
        isfixedStaff: undefined,
        staffName: undefined,
        emailTo: undefined,
        emailCC: undefined,
      };
      this.resetForm("form");
    },
    // 重置查询操作
    resetQuery() {
      this.timeRange = [];
      this.resetForm("queryForm");
      this.handleQuery();
    },
    // 多选框选中数据
    handleSelectionChange(selection) {
      this.ids = selection.map((item) => item.id);
      this.single = selection.length != 1
      this.multiple = !selection.length;
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
          return delCompany(Ids);
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
      getCompany(id).then((res) => {
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
            updateCompany(this.form)
              .then((res) => {
                this.msgSuccess("修改成功");
                this.open = false;
                this.getList();
              })
          } else {
            addCompany(this.form)
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
