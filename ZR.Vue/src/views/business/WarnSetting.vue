<template>
    <div  class="app-container">
        <el-row :gutter="12" class="mb8">
      <el-form  size="small" label-position="right" inline ref="queryForm" label-width="200px"  :model="form" :rules="rules" >
        <el-col >
          <el-form-item label="检测工具每日启动时间" prop="startTime" >
            <el-input v-model="form.startTime" placeholder="请输入时间(HH:MM)" clearable  style="width:300px">
            </el-input>
          </el-form-item>
        </el-col>
        <el-col>
          <el-tag style="margin-left:70px;">注: 请设置每日固定的启动时间，默认为上午八点半</el-tag>
        </el-col>

        <el-col >
          <el-form-item label="异常报警接收邮箱"  prop="mainEmail"  :rules="rules.mainEmail" style="margin-top:10px;">
             <el-input v-model="form.mainEmail" placeholder="请输入有效的邮箱地址"  clearable style="width:300px" />
          </el-form-item>
        </el-col>

        <el-col >
          <el-form-item label="异常报警抄送邮箱1" prop="operationEmailCC1"   :rules="rules.ccEmail1">
            <el-input  v-model="form.operationEmailCC1"  placeholder="请输入有效邮箱地址" clearable style="width:300px" >
            </el-input>
          </el-form-item>
        </el-col>
        <el-col >
          <el-form-item label="异常报警抄送邮箱2" prop="operationEmailCC2"   :rules="rules.ccEmail1">
            <el-input v-model="form.operationEmailCC2" placeholder="请输入有效邮箱地址" clearable style="width:300px">
            </el-input>
          </el-form-item>
        </el-col>
        <el-col >
          <el-form-item label="异常报警抄送邮箱3" prop="operationEmailCC3"   :rules="rules.ccEmail1">
            <el-input v-model="form.operationEmailCC3" placeholder="请输入有效邮箱地址" clearable style="width:300px">
            </el-input>
          </el-form-item>
        </el-col>
        <el-col>

          <el-tag style="margin-left:70px;">注: 邮箱用于自动化出现异常后的及时通报，以便能尽快处理</el-tag>
        </el-col>

        <el-col  :offset="4" style="margin-top:10px;">
          <el-button type="primary" icon="el-icon-search" size="mini" @click="handleSubmit">保存</el-button>
          <el-button icon="el-icon-refresh" size="mini"  @click="handleReset"  >取消</el-button>
        </el-col>

      </el-form>
    </el-row>
    </div>
</template>

<script>

import {
  getRobotList2,
  modifyData,
  getCompany2,
  formatDate,
  updateCompanyEmail
} from '@/api/business/warnSetting.js';
import { MessageBox } from 'element-ui';

  export default {
  data() {
    // 邮箱格式验证正则表达式
    const emailRegex = (rule, value, callback) => {
      if (!value) {
        callback() // 空值直接通过验证
      } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value)) {
        callback(new Error('请输入有效的邮箱地址'))
      } else {
        callback()
      }
    }

    return {
      form: {
            startTime:'08:30',
            mainEmail:'',
            operationEmailCC1:'',
            operationEmailCC2:'',
            operationEmailCC3:''
       },
      rules: {
        mainEmail: [
          { required: true, trigger: ['blur', 'change'] },
          {
            validator: (rule, value, callback) => {
              if (value && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value)) {
                callback(new Error('请输入有效的邮箱地址'));
              } else {
                callback();
              }
            },
            trigger: 'blur'
          }
        ],
        ccEmail1: [
          { validator: emailRegex, trigger: ['blur', 'change'] }
        ],
        ccEmail2: [
          { validator: emailRegex, trigger: ['blur', 'change'] }
        ],
        ccEmail3: [
          { validator: emailRegex, trigger: ['blur', 'change'] }
        ],
        startTime: [
          // 新增必填验证
          {
            required: true,
            message: '检测工具每日启动时间不能为空',
            trigger: 'blur'
          },
          { pattern: /^([0-1]?[0-9]|2[0-3]):([0-5][0-9])$/,
            message: '请输入正确的时间格式（HH:MM）',
            trigger: 'blur'
          }
        ]

      },

      //list1 存放机器人数组
      list1:[],
      //Company对象
      companyRecord:{}
    }
  },
  mounted(){
     this.getData();
  },
  methods:{
    handleSubmit(){

      this.$refs.queryForm.validate(valid => {
        if (!valid) {
          this.$message.error('请完成必填项填写')
          return
        }

        var theTime=formatDate(new Date())+' '+this.form.startTime+':00';
        for(var i=0; i<this.list1.length; i++)
        {
            this.list1[i].startTime=theTime;     //启动时间
        }

        //修改机器人的启动时间
        modifyData(this.list1).then(res=>{
          if(res.code==200)
          {
            //console.log("result5:"+ JSON.stringify(res.data));
          }
        });

        if(this.companyRecord!={})
        {
          //this.form.mainEmail=res.data.operationEmailTo;
          this.companyRecord.operationEmailTo= this.form.mainEmail;
          var operationEmailCCText='';
          if(this.form.operationEmailCC1.length>0)
          {
              operationEmailCCText+=";"+this.form.operationEmailCC1;
          }

          if(this.form.operationEmailCC2.length>0)
          {
              operationEmailCCText+=";"+this.form.operationEmailCC2;
          }

          if(this.form.operationEmailCC3.length>0)
          {
              operationEmailCCText+=";"+this.form.operationEmailCC3;
          }

          if(operationEmailCCText.length>0)
          {
            this.companyRecord.operationEmailCC=operationEmailCCText.substring(1);
          }
          else
          {
            this.companyRecord.operationEmailCC=operationEmailCCText;
          }

          updateCompanyEmail(this.companyRecord).then(res=>{
            if(res.code==200)
            {
                //console.log('result6:'+ JSON.stringify(res.data));
            }
          })

        };

        MessageBox.alert("保存成功!");

      });

    },
    handleReset() {
      this.$refs.queryForm.resetFields()
      // 重置后恢复初始时间
      this.form.startTime = '08:30'
    },

    // 获取配置数据
     getData() {

       getRobotList2().then(res => {
        if (res.code == 200) {
          this.list1=res.data;


          if(this.list1.length>0)
          {
              if(this.list1[0].startTime.length>10)
              {
                //取出来分秒
                var theTime=this.list1[0].startTime.substr(11,5);
                this.form.startTime=theTime;
              }
          }

        }
      })

      //获取id
      var userId = this.$store.getters.userId;
    //获取登录信息
      var userInfo = this.$store.getters.userinfo;
      //console.log("result4:"+ JSON.stringify(userInfo));

      if(userInfo!=null)
      {
          getCompany2({strCompanyId: userInfo.remark }).then(res => {

            if (res.code == 200) {

                  this.companyRecord=res.data;
                  this.form.mainEmail=res.data.operationEmailTo;

                  if(res.data.operationEmailCC!=null && res.data.operationEmailCC.length>0)
                  {
                      var theArray=res.data.operationEmailCC.split(';');
                      for(let i=0; i<theArray.length; i++)
                      {
                          if(i==0)
                          {
                              this.form.operationEmailCC1=theArray[i].trim();
                          }
                          if(i==1)
                          {
                              this.form.operationEmailCC2=theArray[i].trim();
                          }
                          if(i==2)
                          {
                              this.form.operationEmailCC3=theArray[i].trim();
                          }
                      }

                  }

            }
          })

      }

    },


  }
}


</script>

<style>
/* 优化错误提示样式 */
.el-form-item__error {
  position: static;
  margin-top: 5px;
}
</style>
