import request from '@/utils/request'

/**
 * 分页查询
 * @param {查询条件} data
 */
export function listTbResult(query) {
  return request({
    url: 'business/TbResult/list',
    method: 'get',
    params: query,
  })
}


/**
 * 分页查询分组列表
 * @param {查询条件} data
 */
export function listTbResultdistinctlist(query) {
  return request({
    url: 'business/TbResult/distinctlist',
    method: 'get',
    params: query,
  })
}

/**
 * 新增
 * @param data
 */
export function addTbResult(data) {
  return request({
    url: 'business/TbResult',
    method: 'post',
    data: data,
  })
}
/**
 * 修改
 * @param data
 */
export function updateTbResult(data) {
  return request({
    url: 'business/TbResult',
    method: 'PUT',
    data: data,
  })
}
/**
 * 获取详情
 * @param {Id}
 */
export function getTbResult(id) {
  return request({
    url: 'business/TbResult/' + id,
    method: 'get'
  })
}

/**
 * 删除
 * @param {主键} pid
 */
export function delTbResult(pid) {
  return request({
    url: 'business/TbResult/delete/' + pid,
    method: 'POST'
  })
}

/**
 * 获取转发信息
 * @param {主键} pid
 */
export function forwardMessage(pid) {
  return request({
    url: 'business/TbResult/forward/' + pid,
    method: 'POST'
  })
}

/**
 * 复制
 * @param {主键} pid
 */
export function copyMessage(pid) {
  return request({
    url: 'business/TbResult/copy/' + pid,
    method: 'POST'
  })
}

/**
 * 问题件匹配
 * @param data
 */
export function matchResult(data) {
  return request({
    url: 'business/TbResult/match',
    method: 'post',
    data: data,
  })
}
