<!--
 * @Descripttion: (/tb_contact)
 * @version: (1.0)
 * @Author: (root)
 * @Date: (2025-09-03)
 * @LastEditors: (root)
 * @LastEditTime: (2025-09-03)
-->
<template>
  <div class="app-container">
    <el-form :model="queryParams" size="small" label-position="right" inline ref="queryForm" label-width="100px" v-show="showSearch"
      @submit.native.prevent>

      <el-form-item>
        <el-button type="primary" icon="el-icon-search" size="mini" @click="handleQuery">搜索</el-button>
        <el-button icon="el-icon-refresh" size="mini" @click="resetQuery">重置</el-button>
      </el-form-item>
    </el-form>
    <el-row :gutter="10" class="mb8">
      <el-col :span="1.5">
        <el-button type="primary" v-hasPermi="['tbcontact:add']" plain icon="el-icon-plus" size="mini" @click="handleAdd">新增</el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button type="success" :disabled="single" v-hasPermi="['tbcontact:edit']" plain icon="el-icon-edit" size="mini" @click="handleUpdate">修改</el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button type="danger" :disabled="multiple" v-hasPermi="['tbcontact:delete']" plain icon="el-icon-delete" size="mini" @click="handleDelete">删除</el-button>
      </el-col>
      <right-toolbar :showSearch.sync="showSearch" @queryTable="getList"></right-toolbar>
    </el-row>

    <!-- 数据区域 -->
    <el-table :data="dataList" v-loading="loading" ref="table" border highlight-current-row @sort-change="sortChange" @selection-change="handleSelectionChange">
      <el-table-column type="selection" width="50" align="center"/>
      <el-table-column prop="客户" label="客户" align="center" :show-overflow-tooltip="true" />
      <el-table-column prop="客户商家名称" label="客户商家名称" align="center" :show-overflow-tooltip="true" />
      <el-table-column prop="对接方式" label="对接方式" align="center" :show-overflow-tooltip="true" />
      <el-table-column prop="群名称" label="群名称" align="center" :show-overflow-tooltip="true" />
      <el-table-column prop="联系人" label="联系人" align="center" :show-overflow-tooltip="true" />
      <el-table-column prop="是否直接退回" label="是否直接退回" align="center" :show-overflow-tooltip="true" />
      <el-table-column prop="companyId" label="CompanyId" align="center" :show-overflow-tooltip="true" />
      <el-table-column prop="isEnable" label="启用状态：0启用，1禁用" align="center">
        <template slot-scope="scope">
          <dict-tag :options=" isEnableOptions" :value="scope.row.isEnable" />
        </template>
      </el-table-column>
      <el-table-column prop="matchParam" label="匹配参数" align="center" />
      <el-table-column prop="isMatch" label="是否匹配：0启用，1禁用" align="center">
        <template slot-scope="scope">
          <dict-tag :options=" isMatchOptions" :value="scope.row.isMatch" />
        </template>
      </el-table-column>

      <el-table-column label="操作" align="center" width="140">
        <template slot-scope="scope">
          <el-button size="mini" v-hasPermi="['tbcontact:edit']" type="success" icon="el-icon-edit" title="编辑"
            @click="handleUpdate(scope.row)"></el-button>
          <el-button size="mini" v-hasPermi="['tbcontact:delete']" type="danger" icon="el-icon-delete" title="删除"
            @click="handleDelete(scope.row)"></el-button>
        </template>
      </el-table-column>
    </el-table>
    <pagination class="mt10" background :total="total" :page.sync="queryParams.pageNum" :limit.sync="queryParams.pageSize" @pagination="getList" />

    <!-- 添加或修改对话框 -->
    <el-dialog :title="title" :lock-scroll="false" :visible.sync="open" >
      <el-form ref="form" :model="form" :rules="rules" label-width="100px">
        <el-row :gutter="20">

          <el-col :lg="12">
            <el-form-item label="客户" prop="客户">
              <el-input v-model="form.客户" placeholder="请输入客户" />
            </el-form-item>
          </el-col>

          <el-col :lg="12">
            <el-form-item label="客户商家名称" prop="客户商家名称">
              <el-input v-model="form.客户商家名称" placeholder="请输入客户商家名称" />
            </el-form-item>
          </el-col>

          <el-col :lg="12">
            <el-form-item label="对接方式" prop="对接方式">
              <el-input v-model="form.对接方式" placeholder="请输入对接方式" />
            </el-form-item>
          </el-col>

          <el-col :lg="12">
            <el-form-item label="群名称" prop="群名称">
              <el-input v-model="form.群名称" placeholder="请输入群名称" />
            </el-form-item>
          </el-col>

          <el-col :lg="12">
            <el-form-item label="联系人" prop="联系人">
              <el-input v-model="form.联系人" placeholder="请输入联系人" />
            </el-form-item>
          </el-col>

          <el-col :lg="12">
            <el-form-item label="是否直接退回" prop="是否直接退回">
              <el-input v-model="form.是否直接退回" placeholder="请输入是否直接退回" />
            </el-form-item>
          </el-col>

          <el-col :lg="12">
            <el-form-item label="CompanyId" prop="companyId">
              <el-input v-model="form.companyId" placeholder="请输入CompanyId" />
            </el-form-item>
          </el-col>

          <el-col :lg="12">
            <el-form-item label="启用状态：0启用，1禁用" prop="isEnable">
              <el-radio-group v-model="form.isEnable">
                <el-radio v-for="item in isEnableOptions" :key="item.dictValue" :label="item.dictValue">{{item.dictLabel}}</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>

          <el-col :lg="12">
            <el-form-item label="匹配参数" prop="matchParam">
              <el-input v-model="form.matchParam" placeholder="请输入匹配参数" />
            </el-form-item>
          </el-col>

          <el-col :lg="12">
            <el-form-item label="是否匹配：0启用，1禁用" prop="isMatch">
              <el-radio-group v-model="form.isMatch">
                <el-radio v-for="item in isMatchOptions" :key="item.dictValue" :label="item.dictValue">{{item.dictLabel}}</el-radio>
              </el-radio-group>
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
  listTbContact,
  addTbContact,
  delTbContact,
  updateTbContact,
  getTbContact,
} from '@/api/business/tbContact.js';

export default {
  name: "TbContactFullComponent",
  data() {
    return {
      labelWidth: "100px",
      formLabelWidth:"100px",
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
        { index: 0, key: '客户', label: `客户`, checked:  true  },
        { index: 1, key: '客户商家名称', label: `客户商家名称`, checked:  true  },
        { index: 2, key: '对接方式', label: `对接方式`, checked:  true  },
        { index: 3, key: '群名称', label: `群名称`, checked:  true  },
        { index: 4, key: '联系人', label: `联系人`, checked:  true  },
        { index: 5, key: '是否直接退回', label: `是否直接退回`, checked:  true  },
        { index: 6, key: 'companyId', label: `CompanyId`, checked:  true  },
        { index: 7, key: 'isEnable', label: `启用状态：0启用，1禁用`, checked:  true  },
        { index: 8, key: 'matchParam', label: `匹配参数`, checked:  true  },
        { index: 9, key: 'isMatch', label: `是否匹配：0启用，1禁用`, checked:  false  },
      ],
      // 启用状态：0启用，1禁用选项列表 格式 eg:{ dictLabel: '标签', dictValue: '0'}
isEnableOptions: [],
      // 是否匹配：0启用，1禁用选项列表 格式 eg:{ dictLabel: '标签', dictValue: '0'}
isMatchOptions: [],
      dataList: [],
      total: 0,
      rules: {
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
      listTbContact(this.queryParams).then(res => {
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
        客户: undefined,
        客户商家名称: undefined,
        对接方式: undefined,
        群名称: undefined,
        联系人: undefined,
        是否直接退回: undefined,
        companyId: undefined,
        isEnable: undefined,
        matchParam: undefined,
        isMatch: undefined,
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
          return delTbContact(Ids);
        })
        .then(() => {
          this.handleQuery();
          this.msgSuccess("删除成功");
        })
        .catch(() => {});
    },
    /** 修改按钮操作 */
    handleUpdate(row) {
      this.reset();
      const id = row.id || this.ids;
      getTbContact(id).then((res) => {
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
            updateTbContact(this.form)
              .then((res) => {
                this.msgSuccess("修改成功");
                this.open = false;
                this.getList();
            })
          } else {
            addTbContact(this.form)
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
