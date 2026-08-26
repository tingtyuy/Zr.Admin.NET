import request from '@/utils/request'

export function submitTask(data) {
  return request({
    url: '/ai/task/submit',
    method: 'POST',
    data: data,
    headers: { "Content-Type": "multipart/form-data" },
  })
}

export function getTaskStatus(taskNo) {
  return request({
    url: `/ai/task/status/${taskNo}`,
    method: 'GET',
  })
}

export function getTaskList(query) {
  return request({
    url: '/ai/task/list',
    method: 'GET',
    params: query,
  })
}

export function retryTask(taskNo) {
  return request({
    url: `/ai/task/retry/${taskNo}`,
    method: 'POST',
  })
}

export function batchRetryFailed() {
  return request({
    url: '/ai/task/batch-retry',
    method: 'POST',
  })
}

export function deleteTask(taskNo) {
  return request({
    url: `/ai/task/delete/${taskNo}`,
    method: 'POST',
  })
}

export function updateTask(taskNo, data) {
  return request({
    url: `/ai/task/update/${taskNo}`,
    method: 'POST',
    data: data,
  })
}

export function getTemplateList() {
  return request({
    url: '/ai/task/template/list',
    method: 'GET',
  })
}

export function saveTemplate(data) {
  return request({
    url: '/ai/task/template/save',
    method: 'POST',
    data: data,
  })
}

export function deleteTemplate(id) {
  return request({
    url: `/ai/task/template/delete/${id}`,
    method: 'POST',
  })
}

export function batchAddTags(taskNos, tags, removeTags) {
  return request({
    url: '/ai/task/batch-tags',
    method: 'POST',
    data: { taskNos, tags, removeTags },
  })
}

export function batchDownloadResult(taskNos) {
  return request({
    url: '/ai/task/batch-download',
    method: 'POST',
    data: { taskNos },
    responseType: 'blob',
  })
}

export function getStoragePath() {
  return request({
    url: '/ai/task/storage-path',
    method: 'GET',
  })
}

export function getResultImages(params) {
  return request({
    url: '/ai/task/result-images',
    method: 'GET',
    params: params,
  })
}

export function batchMarkPublished(taskNos) {
  return request({
    url: '/ai/task/batch-publish',
    method: 'POST',
    data: { taskNos },
  })
}
