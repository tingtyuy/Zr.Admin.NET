import request from '@/utils/request'

/**
 * 根据公司id查询Robot列表
 * @param {查询条件} data
 */
export function getRobotList(query) {
  return request({
    url: 'business/Robot/dataList',
    method: 'get',
    params: query
  })
}

/**
 * 从系统登录用户获取company, 查询Robot列表
 * @param {查询条件} data
 */
export function getRobotList2() {
  return request({
    url: 'business/Robot/dataList_2',
    method: 'get'
  })
}

/**
 * 获取详情
 *
 */
export function getCompany2(query) {
  return request({
    url: 'business/Company/getData_2',
    method: 'get',
    params:query
  })
}

/**
 * dataList 为List<Robot>
 * 更新数据
 * @param {查询条件} data
 */
export function modifyData(dataList) {
  return request({
    url: 'business/Robot/modify',
    method: 'post',
    data: dataList,
  })
}

/**
 * 更新公司的邮箱
 * @param data
 */
export function updateCompanyEmail(data) {
  return request({
    url: 'business/Company/updateCompanyEmail',
    method: 'post',
    data: data,
  })
}

/**
 * 格式为日期，返回格式yyyy-mm-dd
 * theDate  日期对象
 * 更新数据
 * @param {查询条件} data
 */
export function formatDate(theDate)
{
    var theResult='';
    var monthStr='';
    var dayStr='';
    var theMonth=theDate.getMonth()+1;
    if(theMonth<10)
    {
      monthStr='0'+theMonth;
    }
    else
    {
      monthStr=theMonth;
    }

    if(theDate.getDate()<10)
    {
      dayStr='0'+theDate.getDate();
    }
    else
    {
      dayStr=theDate.getDate();
    }

    theResult=theDate.getFullYear() +'-'+monthStr+'-'+dayStr;
    return theResult;
}

/**
 * 格式为日期，返回格式yyyy-mm-dd HH:mm:ss
 * theDate  日期对象
 * 更新数据
 * @param {查询条件} data
 */
export function formatDate2(theDate)
{
    var theResult='';
    var monthStr='';
    var dayStr='';
    var theMonth=theDate.getMonth()+1;
    if(theMonth<10)
    {
      monthStr='0'+theMonth;
    }
    else
    {
      monthStr=theMonth;
    }

    if(theDate.getDate()<10)
    {
      dayStr='0'+theDate.getDate();
    }
    else
    {
      dayStr=theDate.getDate();
    }

    let hours = String(theDate.getHours()).padStart(2, '0'); // 确保小时为两位数，不足时前面补0
    let minutes = String(theDate.getMinutes()).padStart(2, '0'); // 确保分钟为两位数，不足时前面补0
    let seconds = String(theDate.getSeconds()).padStart(2, '0'); // 确保秒为两位数，不足时前面补0

    theResult=theDate.getFullYear() +'-'+monthStr+'-'+dayStr+" "+hours+":"+minutes+":"+seconds;
    return theResult;
}


/**
 * 获取当月的第一天，小时,分钟，秒都为0. 返回类型为 Date
 * theDate  日期对象
 * 更新数据
 * @param {查询条件} data
 */
export function getMonthFirstDay(theDate)
{
    var date2=new Date();
    date2.setFullYear(theDate.getFullYear());
    date2.setMonth(theDate.getMonth());   
    date2.setDate(1);
    date2.setHours(0);
    date2.setMinutes(0);
    date2.setSeconds(0);
    return date2;
}

/**
 * 获取当月的最后一天，返回类型为 Date, 时、分、秒为23:59:59
 * theDate  日期对象
 * 更新数据
 * @param {查询条件} data
 */
export function getMonthEndDay(theDate)
{
    var date2=new Date();
    date2.setFullYear(theDate.getFullYear());
    date2.setMonth(theDate.getMonth() +1);   
    date2.setDate(1);
    date2.setHours(0);
    date2.setMinutes(0);
    date2.setSeconds(0);

    var myDate=new Date();
    myDate.setTime(date2.getTime()-(86400 * 1 * 1000));   //减1天，获取当月最后一天的日期
    myDate.setHours(23);
    myDate.setMinutes(59);
    myDate.setSeconds(59);
    return myDate;
}