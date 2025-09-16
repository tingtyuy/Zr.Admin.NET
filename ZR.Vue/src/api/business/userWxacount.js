import request from '@/utils/request'

/**
 * 分页查询
 * @param {查询条件} data
 */
export function listUserWxacount(query) {
  return request({
    url: 'business/UserWxacount/list',
    method: 'get',
    params: query,
  })
}

/**
 * 新增
 * @param data
 */
export function addUserWxacount(data) {
  return request({
    url: 'business/UserWxacount',
    method: 'post',
    data: data,
  })
}
/**
 * 修改
 * @param data
 */
export function updateUserWxacount(data) {
  return request({
    url: 'business/UserWxacount',
    method: 'PUT',
    data: data,
  })
}
/**
 * 获取详情
 * @param {Id}
 */
export function getUserWxacount(id) {
  return request({
    url: 'business/UserWxacount/' + id,
    method: 'get'
  })
}

/**
 * 删除
 * @param {主键} pid
 */
export function delUserWxacount(pid) {
  return request({
    url: 'business/UserWxacount/delete/' + pid,
    method: 'POST'
  })
}
