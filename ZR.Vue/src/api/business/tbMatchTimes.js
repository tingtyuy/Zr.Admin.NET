import request from '@/utils/request'

/**
 * 已匹配商户群数量
 * @param {查询条件} data
 */
export function getGroupMatchTimes(query) {
  return request({
    url: 'business/TbMatchTimes/FCount',
    method: 'get',
    params: query
  })
}


