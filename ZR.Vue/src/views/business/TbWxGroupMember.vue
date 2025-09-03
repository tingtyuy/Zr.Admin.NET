<!--
 * @Descripttion: (/tb_wx_group_member)
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
        <el-button type="primary" v-hasPermi="['tbwxgroupmember:add']" plain icon="el-icon-plus" size="mini" @click="handleAdd">新增</el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button type="success" :disabled="single" v-hasPermi="['tbwxgroupmember:edit']" plain icon="el-icon-edit" size="mini" @click="handleUpdate">修改</el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button type="danger" :disabled="multiple" v-hasPermi="['tbwxgroupmember:delete']" plain icon="el-icon-delete" size="mini" @click="handleDelete">删除</el-button>
      </el-col>
      <right-toolbar :showSearch.sync="showSearch" @queryTable="getList"></right-toolbar>
    </el-row>

    <!-- 数据区域 -->
    <el-table :data="dataList" v-loading="loading" ref="table" border highlight-current-row @sort-change="sortChange" @selection-change="handleSelectionChange">
      <el-table-column type="selection" width="50" align="center"/>
      <el-table-column prop="id" label="自增主键" align="center" />
      <el-table-column prop="name" label="姓名" align="center" :show-overflow-tooltip="true" />
      <el-table-column prop="nickName" label="NickName" align="center" :show-overflow-tooltip="true" />
      <el-table-column prop="headPhoto" label="头像base64" align="center">
        <template slot-scope="scope">
          <el-image class="table-td-thumb" fit="contain" :src="scope.row.headPhoto" :preview-src-list="[scope.row.headPhoto]">
            <div slot="error"><i class="el-icon-document" /></div>
          </el-image>
        </template>
      </el-table-column>
      <el-table-column prop="groupName" label="群名称" align="center" :show-overflow-tooltip="true" />
      <el-table-column prop="companyId" label="公司ID" align="center" :show-overflow-tooltip="true" />
      <el-table-column prop="isInternal" label="是否是内部人员" align="center">
        <template slot-scope="scope">
          <dict-tag :options=" isInternalOptions" :value="scope.row.isInternal" />
        </template>
      </el-table-column>

      <el-table-column label="操作" align="center" width="140">
        <template slot-scope="scope">
          <el-button size="mini" v-hasPermi="['tbwxgroupmember:edit']" type="success" icon="el-icon-edit" title="编辑"
            @click="handleUpdate(scope.row)"></el-button>
          <el-button size="mini" v-hasPermi="['tbwxgroupmember:delete']" type="danger" icon="el-icon-delete" title="删除"
            @click="handleDelete(scope.row)"></el-button>
        </template>
      </el-table-column>
    </el-table>
    <pagination class="mt10" background :total="total" :page.sync="queryParams.pageNum" :limit.sync="queryParams.pageSize" @pagination="getList" />

    <!-- 添加或修改对话框 -->
    <el-dialog :title="title" :lock-scroll="false" :visible.sync="open" >
      <el-form ref="form" :model="form" :rules="rules" label-width="100px">
        <el-row :gutter="20">

          <el-col :lg="12" v-if="opertype == 2">
            <el-form-item label="自增主键">{{form.id}}</el-form-item>
          </el-col>

          <el-col :lg="12">
            <el-form-item label="姓名" prop="name">
              <el-input v-model="form.name" placeholder="请输入姓名" />
            </el-form-item>
          </el-col>

          <el-col :lg="12">
            <el-form-item label="NickName" prop="nickName">
              <el-input v-model="form.nickName" placeholder="请输入NickName" />
            </el-form-item>
          </el-col>

          <el-col :lg="24">
            <el-form-item label="头像base64" prop="headPhoto">
              <UploadImage v-model="form.headPhoto" column="headPhoto" @input="handleUploadSuccess" />
            </el-form-item>
          </el-col>

          <el-col :lg="12">
            <el-form-item label="群名称" prop="groupName">
              <el-input v-model="form.groupName" placeholder="请输入群名称" />
            </el-form-item>
          </el-col>

          <el-col :lg="12">
            <el-form-item label="公司ID" prop="companyId">
              <el-input v-model="form.companyId" placeholder="请输入公司ID" />
            </el-form-item>
          </el-col>

          <el-col :lg="12">
            <el-form-item label="是否是内部人员" prop="isInternal">
              <el-radio-group v-model="form.isInternal">
                <el-radio v-for="item in isInternalOptions" :key="item.dictValue" :label="item.dictValue">{{item.dictLabel}}</el-radio>
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
  listTbWxGroupMember,
  addTbWxGroupMember,
  delTbWxGroupMember,
  updateTbWxGroupMember,
  getTbWxGroupMember,
} from '@/api/business/tbWxGroupMember.js';

export default {
  name: "tbwxgroupmember",
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
        { index: 0, key: 'id', label: `自增主键`, checked:  true  },
        { index: 1, key: 'name', label: `姓名`, checked:  true  },
        { index: 2, key: 'nickName', label: `NickName`, checked:  true  },
        { index: 3, key: 'headPhoto', label: `头像base64`, checked:  true  },
        { index: 4, key: 'groupName', label: `群名称`, checked:  true  },
        { index: 5, key: 'companyId', label: `公司ID`, checked:  true  },
        { index: 6, key: 'isInternal', label: `是否是内部人员`, checked:  true  },
      ],
      // 是否是内部人员选项列表 格式 eg:{ dictLabel: '标签', dictValue: '0'}
isInternalOptions: [],
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
      listTbWxGroupMember(this.queryParams).then(res => {
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
        name: undefined,
        nickName: undefined,
        headPhoto: undefined,
        groupName: undefined,
        companyId: undefined,
        isInternal: undefined,
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
          return delTbWxGroupMember(Ids);
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
      getTbWxGroupMember(id).then((res) => {
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
    //图片上传成功方法
    handleUploadSuccess(column, filelist) {
      this.form[column] = filelist;
    },
    /** 提交按钮 */
    submitForm: function () {
      this.$refs["form"].validate((valid) => {
        if (valid) {
          if (this.form.id != undefined && this.opertype === 2) {
            updateTbWxGroupMember(this.form)
              .then((res) => {
                this.msgSuccess("修改成功");
                this.open = false;
                this.getList();
            })
          } else {
            addTbWxGroupMember(this.form)
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
