import request from '@/utils/request'

/**
 * 分页查询
 * @param {查询条件} data
 */
export function listCompany(query) {
  return request({
    url: 'business/Company/list',
    method: 'get',
    params: query,
  })
}

/**
 * 新增
 * @param data
 */
export function addCompany(data) {
  return request({
    url: 'business/Company',
    method: 'post',
    data: data,
  })
}
/**
 * 修改
 * @param data
 */
export function updateCompany(data) {
  return request({
    url: 'business/Company',
    method: 'PUT',
    data: data,
  })
}
/**
 * 获取详情
 * @param {Id}
 */
export function getCompany(id) {
  return request({
    url: 'business/Company/' + id,
    method: 'get'
  })
}

/**
 * 删除
 * @param {主键} pid
 */
export function delCompany(pid) {
  return request({
    url: 'business/Company/delete/' + pid,
    method: 'POST'
  })
}
