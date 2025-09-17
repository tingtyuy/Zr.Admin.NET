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
      <el-row>

        <el-col :span="4">
          <el-button type="primary" plain icon="el-icon-plus" size="mini" @click="handleAdd">新增</el-button>
        </el-col>
      </el-row>
    </el-form>

    <el-table :data="dataList" v-loading="loading" ref="table" border highlight-current-row @row-click="handleRowClick"
      style=" margin-top: 10px;">

      <el-table-column prop="群名称" label="群名称" align="center" :show-overflow-tooltip="true" />


      <!-- <el-table-column label="设置" align="center" width="100">
        <template slot-scope="scope">
          <el-button size="mini" :type="scope.row.isMatch == 1 ? 'info' : 'success'" icon="el-icon-edit"
            @click="handleMatch(scope.row)">设置</el-button>
        </template>
</el-table-column> -->
    </el-table>

    <el-dialog :title="title" :lock-scroll="false" :visible.sync="open" width="20%" :modal="false">
      <el-form ref="form" :model="form" :rules="rules">
        <el-row :span="24">
          <el-col :lg="24">
            <el-form-item label="群名称" prop="群名称">
              <el-input v-model="form.群名称" placeholder="请输入群名称" />
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button type="text" @click="cancel">取 消</el-button>
        <el-button type="primary" @click="submitForm">确 定</el-button>
      </div>
    </el-dialog>

    <el-dialog title="商户群匹配规则设置" :lock-scroll="false" :visible.sync="matchOpen" width="30%" :modal="false">
      <el-form ref="matchForm2" :model="matchForm" label-width="100px">
        <el-row :span="24">
          <el-col :span="12">
            <el-form-item label="群名称" prop="群名称">
              <el-input v-model="matchForm.群名称" placeholder="请输入群名称" />
            </el-form-item>
          </el-col>
          <el-col :lg="12">
            <el-form-item label="私人群" prop="isEnable">
              <el-checkbox v-model="matchForm.isEnable" label="是" />
            </el-form-item>
          </el-col>

          <!-- <el-col :lg="24">
            <el-form-item label="勾选我方人员" prop="mIds">
              <el-select v-model="matchForm.mIds" placeholder="请选择" style="width: 100%;" :multiple="true"
                :clearable="true">
                <el-option v-for="item in WxGroupMemberOptions" :key="item.id" :label="item.nickName"
                  :value="item.id" />
              </el-select>
            </el-form-item>
          </el-col> -->

          <el-col :lg="24">
            <el-form-item label="匹配参数" prop="商户名匹配" style="text-align: left;">
              <el-row :gutter="10">
                <el-col :span="24">
                  <el-checkbox v-model="matchForm.商户名匹配">商户名</el-checkbox>
                </el-col>
              </el-row>
              <el-row :gutter="10">
                <el-col :span="24">
                  <el-checkbox v-model="matchForm.发件人匹配">发件人</el-checkbox>
                </el-col>
              </el-row>
              <el-row :gutter="10">
                <el-col :span="24">
                  <el-checkbox v-model="matchForm.联系电话匹配">联系电话</el-checkbox>
                </el-col>
              </el-row>
              <el-row :gutter="10">
                <el-col :span="24">
                  <el-checkbox v-model="matchForm.地址匹配">地址匹配</el-checkbox>
                </el-col>
              </el-row>

            </el-form-item>
          </el-col>

          <el-col :lg="24">
            <el-form-item label="平台匹配" prop="matchParam" style="text-align: left;">
              <el-select v-model="matchForm.matchParam" placeholder="请选择平台" :clearable="true">
                <el-option v-for="item in isEnableOptions" :key="item.dictValue" :label="item.dictLabel"
                  :value="item.dictValue" />
              </el-select>
            </el-form-item>
          </el-col>

        </el-row>

      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button type="text" @click="matchCancel">取 消</el-button>
        <el-button type="primary" @click="submitMatchForm">确 定</el-button>
      </div>
    </el-dialog>

  </div>
</template>
<script>
import {
  matchTbContact,
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
import { getDicts } from "@/api/system/dict/data";
export default {
  name: "TbContactMatchComponent",
  components: { TbWxGroupMemberComponent },
  props: {
    matchOpen: {
      type: Boolean,
      default: false
    }
  },
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
        群名称: undefined,
        isMatch: undefined,
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
      matchOpen: false,
      // 表单参数
      form: {},
      matchForm: {
        商户名匹配: true,
        发件人匹配: true,
        联系电话匹配: true,
        地址匹配: false,
        mIds: [],
        isEnable: true,
        matchParam: '',
        id: '',
        群名称: '',
      },
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
    // console.log(theme) // 查看所有变量
    this.loadDataSource();
    this.getList();

  },
  methods: {
    handleRowClick(row) {
      this.$emit('rowClick', row);

    },
    tableRowClassName({ row, rowIndex }) {
      if (row.isMatch == 1) {
        return 'success-row';
      }
      return '';
    },
    loadDataSource() {
      getDicts("wx_group_match_param").then((response) => {
        if (response.code == 200) {
          this.isEnableOptions = response.data;
          // this.isEnableOptions = dictData.filter(item => item.dictType === 'is_enable');
          // this.isMatchOptions = dictData.filter(item => item.dictType === 'is_match');
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
    matchCancel() {
      this.matchOpen = false;
      this.WxGroupMemberOptions = [];
      this.matchReset();
      this.$emit('isSet', false);
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
    matchReset() {
      // this.matchForm = {
      //   群名称: undefined,
      //   matchParam: undefined,
      // };
      this.resetForm("matchForm2");
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
    /** 设定匹配规则操作 */
    handleMatch(row) {
      // this.$emit('rowClick', row);
      // this.matchForm={...row};
      this.matchForm.mIds = row.tbWxGroupMembers?.filter(f => f.isInternal == true).map(f => f.id) ?? [];
      this.matchForm.isEnable = row.isEnable;
      this.matchForm.isMatch = row.isMatch;
      this.matchForm.matchParam = row.matchParam;
      this.matchForm.id = row.id;
      this.matchForm.群名称 = row.群名称;
      this.matchOpen = true;
      // listTbWxGroupMemberOptions({
      //   groupName: row.群名称,
      //   ContactId: row.id,
      //   IsInternal: false,
      // }).then((response) => {
      //   if (response.code == 200) {
      //     this.WxGroupMemberOptions = response.data;
      //   }
      // });
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
    /** 设置匹配规则按钮 */
    submitMatchForm: function () {
      var t = this;
      console.log("matchForm data:", JSON.stringify(t.matchForm));
      try {
        matchTbContact(t.matchForm)
          .then((res) => {
            t.msgSuccess("设置匹配规则成功");
            t.matchOpen = false;
            t.$emit('isSet', true);
            t.getList();
          })

      } catch (error) {
          t.$emit('isSet', false);
        console.error("Error in matchTbContact:", error);
        t.msgError("设置匹配规则失败");

      }
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
<style>
.el-table .warning-row {
  background: oldlace;
}

.el-table .success-row {
  background: gray;
}
</style>
