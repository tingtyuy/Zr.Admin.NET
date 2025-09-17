    <template>
      <div class="app-container">
        <el-header class="bordered" style="min-height: 50px;">
          <el-row>
            <el-col :span="6">
              今天问题件处理总数
              <el-tag effect="dark">
                {{ statisticForm.num2 }}
              </el-tag>
            </el-col>
            <el-col :span="6">
              拒收问题件数量
              <el-tag effect="dark">
                {{ statisticForm.num1 }}
              </el-tag>
            </el-col>
            <el-col :span="6">
              破损问题件数量
              <el-tag effect="dark">
                {{ statisticForm.num3 }}
              </el-tag>
            </el-col>
            <el-col :span="6">
              信息有误问题件数量
              <el-tag effect="dark">
                {{ statisticForm.num4 }}
              </el-tag>
            </el-col>
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
            <el-col :span="7" class="bordered height">
              <TbContactComponent ref="leftComponentRef"></TbContactComponent>
            </el-col>
            <el-col :span="17" class="bordered height">
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


export default {

  components: {
    TbContactComponent,
    TbResultComponent
  },
  data() {
    return {
      statisticForm: {
        num1: '999',
        num2: '777',
        num3: '555',
        num4: '333',
        num5: '11',
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
      debugger
      this.$refs.leftComponentRef.getList();
    },
    getList() {
      // this.loading = true;
      // listTbContact(this.wxGroupQueryForm).then(response => {
      //   this.loading = false;
      //   const { data } = response;
      //   this.wxGroupList = data.rows;
      //   this.total = data.total;
      // }).catch(() => {
      //   this.loading = false;
      // });

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

.height {
 max-height: 800px;
 min-height: 660px;
 overflow: scroll;
}
</style>
