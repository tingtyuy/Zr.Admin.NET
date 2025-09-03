    <template>
      <div class="app-container">
        <el-header class="bordered" style="min-height: 100px;">
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
          <el-row>
            <el-col :offset="18" :span="6">
              需要人工处理的问题件数量
              <el-tag color="red" effect="dark">
                {{ statisticForm.num5 }}
              </el-tag>
            </el-col>
          </el-row>

        </el-header>
        <el-main class="bordered">
          <el-row>
            <el-col :span="6" class="bordered height">

              <el-row :gutter="12" class="mb8">
                <el-form :model="wxGroupQueryForm" size="small" label-position="right" inline ref="queryForm"
                  label-width="100px" @submit.native.prevent>

                  <el-col :span="14">
                    <el-form-item label="" prop="name">
                      <el-input v-model="wxGroupQueryForm.name" placeholder="请输入群名称" clearable
                        :style="{ width: '100%' }">
                      </el-input>
                    </el-form-item>
                  </el-col>

                  <el-col :span="10">
                    <el-button type="primary" icon="el-icon-search" size="mini" @click="handleQuery">搜索</el-button>
                    <el-button icon="el-icon-refresh" size="mini" @click="resetQuery">重置</el-button>
                  </el-col>

                </el-form>
              </el-row>
              <el-table :data="wxGroupList" v-loading="loading" ref="table" border highlight-current-row>
                <el-table-column prop="群名称" label="群名称" align="center" :show-overflow-tooltip="true" />
                <el-table-column label="操作" align="center" width="140">
                  <template slot-scope="scope">
                    <el-button size="mini" type="success" icon="el-icon-edit" title="设置匹配规则"
                      @click="handleSetMatchRule(scope.row)"></el-button>
                  </template>
                </el-table-column>
              </el-table>
              <pagination class="mt10" background :total="total" :page.sync="wxGroupList.pageNum"
                :limit.sync="wxGroupList.pageSize" @pagination="getList" />
              <el-dialog title="微信群设置匹配规则弹窗" :lock-scroll="false" :visible.sync="wxGroupDialogOpen">
                <el-form ref="wxGroupDialogForm" :model="wxGroupDialogForm" label-width="100px">
                  <el-row :gutter="20">
                    <el-col :lg="12">
                      <el-form-item label="问题件类型" prop="name">
                        <el-input v-model="wxGroupDialogForm.name" placeholder="请输入问题件类型" />
                      </el-form-item>
                    </el-col>

                  </el-row>
                </el-form>
                <div slot="footer" class="dialog-footer">
                  <el-button type="text" @click="cancel">取 消</el-button>
                  <el-button type="primary" @click="submitForm">确 定</el-button>
                </div>
              </el-dialog>
            </el-col>
            <el-col :span="18" class="bordered height">
              <el-row :gutter="12" class="mb8">
                <el-form :model="wxGroupQueryForm" size="small" label-position="right" inline ref="queryForm"
                  label-width="100px" @submit.native.prevent>

                  <el-col :span="14">
                    <el-form-item label="" prop="name">
                      <el-input v-model="wxGroupQueryForm.name" placeholder="请输入群名称" clearable
                        :style="{ width: '100%' }">
                      </el-input>
                    </el-form-item>
                  </el-col>

                  <el-col :span="10">
                    <el-button type="primary" icon="el-icon-search" size="mini" @click="handleQuery">搜索</el-button>
                    <el-button icon="el-icon-refresh" size="mini" @click="resetQuery">重置</el-button>
                  </el-col>

                </el-form>
              </el-row>
              <el-table :data="wxGroupList" v-loading="loading" ref="table" border highlight-current-row>
                <el-table-column prop="群名称" label="群名称" align="center" :show-overflow-tooltip="true" />
                <el-table-column label="操作" align="center" width="140">
                  <template slot-scope="scope">
                    <el-button size="mini" type="success" icon="el-icon-edit" title="设置匹配规则"
                      @click="handleUpdate(scope.row)"></el-button>
                  </template>
                </el-table-column>
              </el-table>
              <pagination class="mt10" background :total="total" :page.sync="wxGroupList.pageNum"
                :limit.sync="wxGroupList.pageSize" @pagination="getList" />
            </el-col>
          </el-row>
        </el-main>
      </div>
    </template>
<script>
export default {
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
  },
  methods: {
    getList() {
      this.loading = true;
      setTimeout(() => {
        this.loading = false;
        this.total = 1;
      }, 500);
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
  min-height: 550px;
}
</style>
