import request from '@/utils/request'

/**
 * 分页查询
 * @param {查询条件} data
 */
export function listTbContact(query) {
  return request({
    url: 'business/TbContact/list',
    method: 'get',
    params: query,
  })
}

/**
 * 分页查询2
 * @param {查询条件} data
 */
export function listTbContact2(query) {
  return request({
    url: 'business/TbContact/list2',
    method: 'get',
    params: query,
  })
}

/**
 * 新增
 * @param data
 */
export function addTbContact(data) {
  return request({
    url: 'business/TbContact',
    method: 'post',
    data: data,
  })
}

/**
 * / 商户群列表修改私人群状态和群名称
 * @param data
*/
export function updateTbContact2(data) {
  return request({
    url: 'business/TbContact/update',
    method: 'PUT',
    data: data,
  })
}
/**
 * 修改
 * @param data
*/
export function updateTbContact(data) {
  return request({
    url: 'business/TbContact',
    method: 'PUT',
    data: data,
  })
}
/**
 * 设定匹配规则
 * @param data
 */
export function matchTbContact(data) {
  return request({
    url: 'business/TbContact/matchByWeiXinGroup',
    method: 'PUT',
    data: data,
  })
}
/**
 * 获取详情
 * @param {Id}
 */
export function getTbContact(id) {
  return request({
    url: 'business/TbContact/' + id,
    method: 'get'
  })
}

/**
 * 删除
 * @param {主键} pid
 */
export function delTbContact(pid) {
  return request({
    url: 'business/TbContact/delete/' + pid,
    method: 'POST'
  })
}
