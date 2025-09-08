import request from '@/utils/request'

/**
 * 分页查询
 * @param {查询条件} data
 */
export function listTbWxGroupMember(query) {
  return request({
    url: 'business/TbWxGroupMember/list',
    method: 'get',
    params: query,
  })
}

/**
 * Opitons
 */
export function listTbWxGroupMemberOptions() {
  return request({
    url: 'business/TbWxGroupMember/options',
    method: 'get'
  })
}


/**
 * 新增
 * @param data
 */
export function addTbWxGroupMember(data) {
  return request({
    url: 'business/TbWxGroupMember',
    method: 'post',
    data: data,
  })
}
/**
 * 修改
 * @param data
 */
export function updateTbWxGroupMember(data) {
  return request({
    url: 'business/TbWxGroupMember',
    method: 'PUT',
    data: data,
  })
}
/**
 * 获取详情
 * @param {Id}
 */
export function getTbWxGroupMember(id) {
  return request({
    url: 'business/TbWxGroupMember/' + id,
    method: 'get'
  })
}

/**
 * 删除
 * @param {主键} pid
 */
export function delTbWxGroupMember(pid) {
  return request({
    url: 'business/TbWxGroupMember/delete/' + pid,
    method: 'POST'
  })
}
