<template>
  <div class="dashboard-container">
    <!-- 日期选择 -->
    <div class="date-picker">
      <span class="date-label">日期: {{ dateStr }}</span>
    </div>

    <!-- 月度概览卡片 -->
    <div class="overview-cards">
      <div class="overview-card">
        <div class="card-header">
          <span class="card-title">问题件总数</span>
          <span class="card-subtitle">日均</span>
        </div>
        <div class="card-content">
          <div class="card-value text-highlight">{{ monthlyProblemOrder.probleCount }}</div>
          <div class="card-meta">{{ monthlyProblemOrder.dailyOrder }} <span class="unit">件/天</span></div>
        </div>
      </div>

	<div class="overview-card">
        <div class="card-header">
          <span class="card-title">问题件总数</span>
          <span class="card-subtitle">日均</span>
        </div>
        <div class="card-content">
          <div class="card-value text-highlight">{{ monthlyProblemOrder.probleCount }}</div>
          <div class="card-meta">{{ monthlyProblemOrder.dailyOrder }} <span class="unit">件/天</span></div>
        </div>
      </div>

	<div class="overview-card">
        <div class="card-header">
          <span class="card-title">自动化处理总数</span>
          <span class="card-subtitle">日均</span>
        </div>
        <div class="card-content">
          <div class="card-value text-highlight">{{  monthlyProcessOrder.TotalNumber}}</div>
          <div class="card-meta">{{ monthlyProcessOrder.dailyOrder }} <span class="unit">件/天</span></div>
        </div>
      </div>

	<div class="overview-card">
        <div class="card-header">
          <span class="card-title">自动化处理占比</span>
         </div>
        <div class="card-content">
          <div class="card-value text-highlight">{{ processPercent }}</div>
        </div>
      </div>

        <div class="overview-card">
        <div class="card-header">
          <span class="card-title">自动化处理总耗时(h)</span>
          <span class="card-subtitle">日均(h)</span>
        </div>
        <div class="card-content">
          <div class="card-value text-highlight">{{ monthlyProcessOrder.totalUseTime }}</div>
          <div class="card-meta">{{ monthlyProcessOrder.dailyUseTime }} <span class="unit">件/天</span></div>
        </div>
      </div>

    </div>

    <!-- 商户统计 -->
    <div class="merchant-section bordered">
      <h3 class="section-title">商户群统计</h3>
      <div class="stat-grid">
        <div class="stat-item">
          <div class="stat-label">商户群总数</div>
          <div class="stat-value text-highlight">{{ weiXinGroup.totalNumber }}</div>
        </div>

	<div class="stat-item">
          <div class="stat-label">已匹配数</div>
          <div class="stat-value text-highlight">{{ weiXinGroup.matchedNumber }}</div>
        </div>

	<div class="stat-item">
          <div class="stat-label">待匹配数</div>
          <div class="stat-value text-highlight">{{ weiXinGroup.uNMatchedNumber }}</div>
        </div>

	<div class="stat-item">
          <div class="stat-label">本月新增数</div>
          <div class="stat-value text-highlight">{{ weiXinGroup.newGroupNumber }}</div>
        </div>

      </div>

      <div class="ranking-section">
        <h4>本月前五排名</h4>
        <div class="ranking-list">
          <div 
            v-for="(item, index) in topRanking" 
            :key="index"
            class="ranking-item"
            :style="{ backgroundColor: getRankColor(index) }"
          >
            <span class="rank-index">{{ index + 1 }}</span>
            <span class="rank-name">{{ item }}</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>

import {
  getProcessOrder,
  getMonthlyProblemOrder,
  getWeiXinGroupNumber_2,
  getMonthlyProcessOrder,
  getDifferenceDays,
  roundData,
  getTop5Group 
} from '@/api/business/dataStatistics.js';

import {
  formatDate, 
  formatDate2, 
  getMonthFirstDay, 
  getMonthEndDay
} from '@/api/business/warnSetting.js';

export default {
  data() {
    return {
      dateStr: '',
      
	//昨天的日期
      yesterdayStr:'',
       //昨日处理的问题件
      forder:{
        TotalNumber:null,
        FReject:null,
        FDamage:null,
        Funknown:null,
        Fother:null
      },
       //从网点管家获取的问题件月度统计
      monthlyProblemOrder:{
        //网点管家中全部的问题件数量
        allcount:null,
        //网点管家中仅查询的问题件数量
        checkcount:null,
        //网点管家中仅通知的问题件数量
        noticecount:null,
        //机器人可以处理的问题件数量
        probleCount:null,        
        //日均
        dailyOrder:null
      },
      //月度处理的问题件
      monthlyProcessOrder:{
        TotalNumber:null,
        //日均处理问题件
        dailyOrder:null,
        //自动化处理总耗时
        totalUseTime:null,
        //日均耗时
        dailyUseTime:null        
      },
      processPercent:null,
      //微信群数量
      weiXinGroup:{
        totalNumber:null,
        matchedNumber:null,
        uNMatchedNumber:null,
        //本月新增的微信群
        newGroupNumber:null,
        top5Group1:null,
        top5Group2:null,
        top5Group3:null,
        top5Group4:null,
        top5Group5:null
      },
      topRanking: [
        this.weiXinGroup.top5Group1,
        this.weiXinGroup.top5Group2,
        // ...其他排名
      ]
    }
  },
  methods: {
    formatLabel(key) {
      const labelMap = {
        TotalNumber: '处理总数',
        FReject: '拒收',
        // ...其他映射
      }
      return labelMap[key] || key
    },
    getRankColor(index) {
      const colors = ['#409EFF', '#67C23A', '#E6A23C', '#F56C6C', '#909399']
      return colors[index]
    }
  },
  mounted() {
      this.dateStr = formatDate(new Date());
    
    //昨天的日期
      var theYesterday=new Date();
      theYesterday.setTime(theYesterday.getTime() -(86400 * 1 * 1000));     //日期减1
      theYesterday.setHours(23, 59, 59);

      this.yesterdayStr=formatDate(theYesterday );

      var startDateForMonth=getMonthFirstDay(new Date());   //当月第一天
     
     
      var theDate1=new Date(formatDate(startDateForMonth ));    //去掉时、分、秒      
      var fdays=getDifferenceDays(theDate1, theYesterday)+1;        //天数，用于算日均，统计截止到昨天

      //获取id
      var userId = this.$store.getters.userId;
      //获取登录信息
      var userInfo = this.$store.getters.userinfo;
   
      var theParam={"strDate":formatDate(theYesterday ), "strCompanyId": userInfo.remark};

      //获取昨日处理的问题件
      getProcessOrder(theParam).then(res => {   
        
        if (res.code == 200) {          
          this.forder.TotalNumber=res.data.totalNumber;
          this.forder.FReject=res.data.fReject;
          this.forder.FDamage=res.data.fDamage;
          this.forder.Funknown=res.data.funknown;
          this.forder.Fother=res.data.fOther;
        }
      });

     var theParam2={"strStartDate":formatDate(startDateForMonth ), "strCompanyId": userInfo.remark, "strEndDate":this.yesterdayStr }; 
      //从网点管家获取的问题件月度统计，统计日期截止到昨天
      getMonthlyProblemOrder(theParam2).then(res => {   
        
        if (res.code == 200 && res.data!=null) {  

          this.monthlyProblemOrder.probleCount=res.data.allcount -res.data.checkcount -res.data.noticecount;      
           if(fdays>0)
            {
                this.monthlyProblemOrder.dailyOrder= roundData(this.monthlyProblemOrder.probleCount/fdays,2);                
            } 
        }
      });

      //自动化工具月度处理的数量,统计日期截止到昨天
      var theParam3={"strStartDate":formatDate2(startDateForMonth), "strCompanyId": userInfo.remark, "strEndDate":formatDate2(theYesterday) };    
      getMonthlyProcessOrder(theParam3).then(res=>{
          
          if(res.code==200)
          {
              this.monthlyProcessOrder.TotalNumber=res.data.totalNumber;    
              this.monthlyProcessOrder.totalUseTime=roundData(res.data.totalUseTime/3600, 1);     //转化为小时 
              if(this.monthlyProblemOrder.probleCount>0 )
              {
                this.processPercent= roundData(res.data.totalNumber/this.monthlyProblemOrder.probleCount*100, 1);
                if(fdays>0)
                {
                    this.monthlyProcessOrder.dailyOrder= roundData(res.data.totalNumber/fdays,2);
                    this.monthlyProcessOrder.dailyUseTime=roundData(res.data.totalUseTime/3600/fdays, 1);     //日均耗时(h)
                }
                
              }         
          }

      })

       //微信群数量统计
      getWeiXinGroupNumber_2({"strDate":this.yesterdayStr}).then(res => {   
        
        if (res.code == 200 && res.data!=null) {  
            if(res.data.length>0)
            {
                this.weiXinGroup.totalNumber=res.data[0].totalNumber;
                this.weiXinGroup.matchedNumber=res.data[0].matchedNumber;
                this.weiXinGroup.uNMatchedNumber=res.data[0].unMatchedNumber;   
                

                if(res.data[0].lastMonthNumber>0 && res.data[0].totalNumber>0)
                {
                   this.weiXinGroup.newGroupNumber= res.data[0].totalNumber - res.data[0].lastMonthNumber;
                }
            }        
               
        }
      })

      //月处理量的前5名
      var top5Param={"strStartDate":formatDate2(startDateForMonth), "strEndDate":formatDate2(theYesterday) };  
      getTop5Group(top5Param).then(res=>{        
        if(res.code==200 && res.data!=null)
        {
            var theArray=res.data;
            for(var i=0; i<theArray.length; i++)
            {
                if(i==0)
                {
                  this.weiXinGroup.top5Group1=theArray[i].groupName;
                }
                else if(i==1)
                {
                  this.weiXinGroup.top5Group2=theArray[i].groupName;
                }
                 else if(i==2)
                {
                  this.weiXinGroup.top5Group3=theArray[i].groupName;
                }
                 else if(i==3)
                {
                  this.weiXinGroup.top5Group4=theArray[i].groupName;
                }
                 else if(i==4)
                {
                  this.weiXinGroup.top5Group5=theArray[i].groupName;
                }                
            }
        }        

      })

  }
}
</script>

<style scoped>
:root {
  --primary-color: #1890ff;
  --success-color: #52c41a;
  --warning-color: #faad14;
  --error-color: #ff4d4f;
  --card-bg: #ffffff;
  --border-color: #f0f0f0;
}

.dashboard-container {
  padding: 24px;
  background-color: #f5f5f5;
}

/* 概览卡片样式 */
.overview-cards {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: 20px;
  margin: 20px 0;
}

.overview-card {
  background: var(--card-bg);
  border-radius: 8px;
  padding: 20px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
  border-left: 4px solid var(--primary-color);
}

.card-header {
  display: flex;
  justify-content: space-between;
  margin-bottom: 12px;
}

.card-title {
  font-weight: 600;
  color: #595959;
}

.card-subtitle {
  font-size: 12px;
  color: #8c8c8c;
}

.card-value {
  font-size: 24px;
  font-weight: bold;
}

.unit {
  font-size: 12px;
  color: #8c8c8c;
  margin-left: 4px;
}

/* 数据区域样式 */
.data-section, .merchant-section {
  background: white;
  border-radius: 8px;
  padding: 20px;
  margin-bottom: 20px;
}

.section-title {
  font-size: 16px;
  font-weight: 600;
  padding-bottom: 12px;
  border-bottom: 1px solid var(--border-color);
}

.data-grid, .stat-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(120px, 1fr));
  gap: 16px;
  margin-top: 16px;
}

.data-item, .stat-item {
  padding: 12px;
  background: #fafafa;
  border-radius: 6px;
  text-align: center;
}

.data-label, .stat-label {
  font-size: 12px;
  color: #8c8c8c;
  margin-bottom: 4px;
}

.data-value, .stat-value {
  font-weight: 600;
  font-size: 18px;
}

/* 排名样式 */
.ranking-section {
  margin-top: 20px;
}

.ranking-list {
  margin-top: 8px;
}

.ranking-item {
  display: flex;
  align-items: center;
  padding: 10px 12px;
  border-radius: 4px;
  margin-bottom: 8px;
  background-color: #f5f5f5;
}

.rank-index {
  width: 24px;
  height: 24px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 4px;
  margin-right: 12px;
  font-weight: 600;
  color: white;
}

.rank-name {
  font-size: 14px;
}

/* 文本样式 */
.text-highlight {
  color: var(--primary-color);
}

.bordered {
  border: 1px solid var(--border-color);
  border-radius: 8px;
}
</style>