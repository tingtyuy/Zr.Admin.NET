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
    <el-form :model="queryParams" size="small" label-position="right" inline ref="queryForm" label-width="100px"
      v-show="showSearch" @submit.native.prevent>

      <el-form-item>
        <el-button type="primary" icon="el-icon-search" size="mini" @click="handleQuery">搜索</el-button>
        <el-button icon="el-icon-refresh" size="mini" @click="resetQuery">重置</el-button>
      </el-form-item>
    </el-form>

    <!-- 数据区域 -->
    <el-table :data="dataList" v-loading="loading" ref="table" border highlight-current-row>

      <el-table-column prop="群名称" label="群名称" align="center" :show-overflow-tooltip="true" width="240" />
      <!-- <el-table-column prop="isEnable" label="启用状态" align="center" width="50" >
        <template slot-scope="scope">
          <dict-tag :options=" isEnableOptions" :value="scope.row.isEnable" />
        </template>
</el-table-column> -->
      <el-table-column prop="isMatch" label="状态" align="center" width="60">

        <template slot-scope="scope">
          {{ scope.row.isMatch == 1 ? '已匹配' : '未匹配' }}
          <!-- <dict-tag :options=" isMatchOptions" :value="scope.row.isMatch" /> -->
        </template>
      </el-table-column>

      <el-table-column label="操作" align="center" width="60">
        <template slot-scope="scope">
          <el-button size="mini" type="success" icon="el-icon-edit" title="编辑"
            @click="handleAdd(scope.row)"></el-button>
        </template>
      </el-table-column>
    </el-table>
    <!-- <pagination small class="mt2" background :total="total" :page.sync="queryParams.pageNum" :limit.sync="queryParams.pageSize" @pagination="getList" /> -->

    <!-- 添加或修改对话框 -->
    <el-dialog title="微信群设置匹配规则弹窗" :lock-scroll="false" :visible.sync="open" width="30%">
      <el-form ref="form" :model="form" :rules="rules" label-width="100px">
        <el-row :span="24">
          <el-col :span="12">
            <el-form-item label="群名称" prop="群名称">
              <el-input v-model="form.群名称" placeholder="请输入群名称" />
            </el-form-item>
          </el-col>
          <el-col :lg="12">
            <el-form-item label="私人群" prop="isEnable">
              <el-checkbox v-model="form.isEnable" label="是" true-label="0" false-label="1" />
            </el-form-item>
          </el-col>
          <el-col :lg="24">
            <el-form-item label="勾选我方人员" prop="members">
              <el-select v-model="form.members" placeholder="请选择" style="width: 100%;" :multiple="true" :clearable="true">
                <el-option v-for="item in WxGroupMemberOptions" :key="item.id" :label="item.nickName"
                  :value="item.id" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :lg="24">
            <el-form-item label="匹配参数" prop="matchParam">
              <el-select v-model="form.matchParam" placeholder="请选择" style="width: 100%;" :multiple="true" :clearable="true">
                <el-option v-for="item in isEnableOptions" :key="item.dictValue" :label="item.dictLabel"
                  :value="item.dictValue" />
              </el-select>
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
import {
  listTbWxGroupMemberOptions,
  listTbWxGroupMember,
  addTbWxGroupMember,
  delTbWxGroupMember,
  updateTbWxGroupMember,
  getTbWxGroupMember,
} from '@/api/business/tbWxGroupMember.js';
import TbWxGroupMemberComponent from '@/views/business/TbWxGroupMemberComponent.vue';
// import dictData from '@/views/components/dictData'
import { getDicts } from "@/api/system/dict/data";
export default {
  name: "TbContactComponent",
  components: { TbWxGroupMemberComponent },
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
        pageSize: 9999,
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
        { index: 0, key: '客户', label: `客户`, checked: true },
        { index: 1, key: '客户商家名称', label: `客户商家名称`, checked: true },
        { index: 2, key: '对接方式', label: `对接方式`, checked: true },
        { index: 3, key: '群名称', label: `群名称`, checked: true },
        { index: 4, key: '联系人', label: `联系人`, checked: true },
        { index: 5, key: '是否直接退回', label: `是否直接退回`, checked: true },
        { index: 6, key: 'companyId', label: `CompanyId`, checked: true },
        { index: 7, key: 'isEnable', label: `启用状态：0启用，1禁用`, checked: true },
        { index: 8, key: 'matchParam', label: `匹配参数`, checked: true },
        { index: 9, key: 'isMatch', label: `是否匹配：0启用，1禁用`, checked: false },
      ],
      // 启用状态：0启用，1禁用选项列表 格式 eg:{ dictLabel: '标签', dictValue: '0'}
      isEnableOptions: [],
      // 是否匹配：0启用，1禁用选项列表 格式 eg:{ dictLabel: '标签', dictValue: '0'}
      isMatchOptions: [],
      WxGroupMemberOptions: [],
      dataList: [],
      total: 0,
      rules: {
      },
    };
  },
  created() {
    this.loadDataSource();
    this.getList();

  },
  methods: {
    loadDataSource() {
      getDicts("wx_group_match_param").then((response) => {
        if (response.code == 200) {
          this.isEnableOptions = response.data;
          // this.isEnableOptions = dictData.filter(item => item.dictType === 'is_enable');
          // this.isMatchOptions = dictData.filter(item => item.dictType === 'is_match');
        }
      });
      listTbWxGroupMemberOptions().then((response) => {
        if (response.code == 200) {
          this.WxGroupMemberOptions = response.data;
        }
      });
    },
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
