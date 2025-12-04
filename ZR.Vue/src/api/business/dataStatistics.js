import request from '@/utils/request'

/**
 * 获取昨日的问题件数量，这是自动化工具处理过的问题件
 *
 */
export function getProcessOrder(query) {
  return request({
    url: 'business/TbOrder/dailyData',
    method: 'get',
    params:query
  })
}

/**
 * 获取一个时间段的问题件数量，这是自动化工具处理过的问题件
 *
 */
export function getMonthlyProcessOrder(query) {
  return request({
    url: 'business/TbOrder/monthlyData_2',
    method: 'get',
    params:query
  })
}

/**
 * 从网点管家获取问题件数量
 *
 */
export function getMonthlyProblemOrder(query) {
  return request({
    url: 'business/FDataCount/dataRow',
    method: 'get',
    params:query
  })
}

/**
 * 获取微信群的数量，从tb_order 表现统计
 *
 */
export function getWeiXinGroupNumber() {
  return request({
    url: 'business/TbContact/totalGroupNumber',
    method: 'get'
  })
}

/**
 * 获取微信群的数量，从数据表统计
 *
 */
export function getWeiXinGroupNumber_2(query) {
  return request({
    url: 'business/TbContact/totalGroupNumber_2',
    method: 'get',
    params:query
  })
}

/**
 * 获取问题件处理前5的微信群
 *
 */
export function getTop5Group(query) {
  return request({
    url: 'business/TbResult/top5Group',
    method: 'get',
    params:query
  })
}

/**
 * 计算date2 与date1的差值天数，返回天数。date2- date1
 *
 */
export function getDifferenceDays(date1, date2) {
  // 计算两个日期之间的差异（毫秒）
  var diff = date2.getTime() - date1.getTime();

  // 将毫秒转换为天
  var diffDays = diff / (1000 * 3600 * 24);
  return diffDays;
}

/**
 * 对数据保留小数
 * decimalNumber  小数位数 
 */
export function roundData(theVlue, decimalNumber) {
  // 计算两个日期之间的差异（毫秒）
  var theData=Math.round(theVlue* Math.pow(10, decimalNumber)) /Math.pow(10, decimalNumber);
  return theData;
}
