    <template>
      <div class="app-container">
        <el-header class="bordered" style="min-height: 100px; text-align: left;padding-left: 60px;">
                    <el-row  style="font-size: 20px; font-weight: bold;">
            <el-col :span="6">
              本月问题件总数
              <el-tag effect="dark" size="medium"style="margin-left: 10px;">
                {{ statisticForm.count1 }}
              </el-tag>
            </el-col>
            <el-col :span="6">
              处理总数
              <el-tag effect="dark" size="medium"style="margin-left: 10px;">
                {{ statisticForm.count2 }}
              </el-tag>
            </el-col>
            <el-col :span="6">
              占比
              <el-tag effect="dark" size="medium"style="margin-left: 10px;">
                {{ statisticForm.count3 }}
              </el-tag>
            </el-col>
            <el-col :span="6" >
              日均
              <el-tag effect="dark" size="medium"style="margin-left: 10px;">
                {{ statisticForm.count4 }}
              </el-tag>
            </el-col>
            <!-- <el-col :span="6">
              信息有误问题件数量
              <el-tag effect="dark">
                {{ statisticForm.num4 }}
              </el-tag>
            </el-col> -->
          </el-row>
          <el-row style="margin-top: 20px;">
            <el-col :span="6">
              今天问题件处理总数
              <el-tag effect="dark" size="medium" style="margin-left: 10px;">
                {{ statisticForm.sum }}
              </el-tag>
            </el-col>
            <el-col :span="6">
              拒收问题件数量
              <el-tag effect="dark" size="medium"style="margin-left: 10px;">
                {{ statisticForm.ju }}
              </el-tag>
            </el-col>
            <el-col :span="6">
              破损问题件数量
              <el-tag effect="dark" size="medium"style="margin-left: 10px;">
                {{ statisticForm.po }}
              </el-tag>
            </el-col>
            <el-col :span="6">
              已匹配商户群数量
              <el-tag effect="dark" size="medium"style="margin-left: 10px;">
                {{ statisticForm.sendSum }}
              </el-tag>
            </el-col>
            <!-- <el-col :span="6">
              信息有误问题件数量
              <el-tag effect="dark">
                {{ statisticForm.num4 }}
              </el-tag>
            </el-col> -->
          </el-row>
          <!-- <el-row>
            <el-col :offset="18" :span="6">
              需要人工处理的问题件数量
              <el-tag color="red" effect="dark">
                {{ statisticForm.num5 }}
              </el-tag>
            </el-col>
          </el-row> -->

        </el-header>
        <el-main class="bordered">
          <el-row>
            <el-col :span="7" class="bordered">
              <TbContactComponent ref="leftComponentRef"></TbContactComponent>
            </el-col>
            <el-col :span="17" class="bordered">
              <TbResultComponent @refreshLeftList="refreshLeftListCallBack"></TbResultComponent>
            </el-col>
          </el-row>
        </el-main>
      </div>
    </template>
<script>
import TbContactComponent from '@/views/business/TbContactComponent.vue';
import TbResultComponent from '@/views/business/TbResultComponent.vue';
import {
  getStatistic,
  listTbResultdistinctlist,
  listTbResult,
  addTbResult,
  delTbResult,
  updateTbResult,
  getTbResult,
  forwardMessage,
  copyMessage,
} from '@/api/business/tbResult.js';

import {
  listTbContact,
  addTbContact,
  delTbContact,
  updateTbContact,
  getTbContact,
} from '@/api/business/tbContact.js';

import {
  getGroupMatchTimes
} from '@/api/business/tbMatchTimes.js';

import {
  formatDate
} from '@/api/business/warnSetting.js';

import {
  getProcessOrder
} from '@/api/business/dataStatistics.js';

export default {

  components: {
    TbContactComponent,
    TbResultComponent
  },
  data() {
    return {
      statisticForm: {
        sum: 0,
        ju: 0,
        po: 0,
        sendSum: 0,
        count1:0,
        count2:0,
        count3:0,
        count4:0

      },
      wxGroupDialogOpen: false,
      wxGroupDialogForm: {
        name: '',
      },
      wxGroupQueryForm: {
        name: '',
        pageNum: 1,
        pageSize: 10
      },
      wxGroupList:
        [
          { 群名称: '测试群' }
        ]


    }
  }
  ,
  created() {
    // 页面列表数据查询
    this.getList();
  },
  methods: {
    refreshLeftListCallBack() {
      this.getList();
      this.$refs.leftComponentRef.getList();
    },
    getList() {

         //获取id
      var userId = this.$store.getters.userId;
      //获取登录信息
      var userInfo = this.$store.getters.userinfo;

      getStatistic().then(response => {
        const { data } = response;

        this.statisticForm.sum=data.sum;
        this.statisticForm.ju=data.ju;
        this.statisticForm.po=data.po;

        this.statisticForm.count1=data.count1;
        this.statisticForm.count2=data.count2;
        this.statisticForm.count3=data.count3;
        this.statisticForm.count4=data.count4;

      });

      //已匹配商户群数量
      var theParam1={"strDate": formatDate(new Date()), "strUserAccount": userInfo.userName};

      getGroupMatchTimes(theParam1).then(
        response => {
          //console.log("getGroupMatchTimes():"+ JSON.stringify(response ));
          if(response.code == 200)
          {
              //已匹配商户群数量
              this.statisticForm.sendSum=response.data;
          }

        }
      )

    },
    handleQuery() {
      this.wxGroupQueryForm.pageNum = 1;
      this.getList();
    },
    resetQuery() {
      this.wxGroupQueryForm = {
        name: '',
        pageNum: 1,
        pageSize: 10
      };
      this.getList();
    },
    handleSetMatchRule(row) {
      this.wxGroupDialogOpen = true;
      this.wxGroupDialogForm = Object.assign({}, row);
    },
    handleUpdate(row) {
      this.wxGroupDialogOpen = true;
      this.wxGroupDialogForm = Object.assign({}, row);
    },
    cancel() {
      this.wxGroupDialogOpen = false;
      this.$refs['wxGroupDialogForm'].resetFields();
    },
    submitForm() {
      this.$refs['wxGroupDialogForm'].validate((valid) => {
        if (valid) {
          this.$message.success('操作成功');
          this.wxGroupDialogOpen = false;
          this.getList();
        } else {
          return false;
        }
      });
    }

  },
}
</script>
<style lang="css" scoped>
.bordered {
  border: 1px solid #eee;
  padding: 10px 0;
  margin-bottom: 10px;
  text-align: center;
}
</style>
