import request from '@/utils/request'

// ===== 配置 =====
export function getComfyuiConfig() {
  return request({
    url: '/comfyui/config',
    method: 'GET'
  })
}

export function saveComfyuiConfig(data) {
  return request({
    url: '/comfyui/config',
    method: 'POST',
    data: data
  })
}

export function testComfyuiConfig(data) {
  return request({
    url: '/comfyui/config/test',
    method: 'POST',
    data: data
  })
}

// ===== 工作流 =====
export function importWorkflows(data) {
  return request({
    url: '/comfyui/workflow/import',
    method: 'POST',
    data: data
  })
}

export function getWorkflowList(query) {
  return request({
    url: '/comfyui/workflow/list',
    method: 'GET',
    params: query
  })
}

export function getWorkflowDetail(id) {
  return request({
    url: `/comfyui/workflow/detail/${id}`,
    method: 'GET'
  })
}

export function getWorkflowVariables(id) {
  return request({
    url: `/comfyui/workflow/variables/${id}`,
    method: 'GET'
  })
}

export function getEditableNodes(id) {
  return request({
    url: `/comfyui/workflow/${id}/editable-nodes`,
    method: 'GET'
  })
}

export function setWorkflowCategory(data) {
  return request({
    url: '/comfyui/workflow/category',
    method: 'POST',
    data: data
  })
}

export function deleteWorkflow(id) {
  return request({
    url: `/comfyui/workflow/delete/${id}`,
    method: 'POST'
  })
}

export function updateComfyuiWorkflow(id, data) {
  return request({
    url: `/comfyui/workflow/update/${id}`,
    method: 'POST',
    data: data
  })
}

export function updateWorkflowVariables(id, variableNodes) {
  return request({
    url: `/comfyui/workflow/variables/${id}`,
    method: 'POST',
    data: { variableNodes: variableNodes }
  })
}

// ===== 任务 =====
export function createComfyuiTask(data) {
  return request({
    url: '/comfyui/task/create',
    method: 'POST',
    data: data,
    timeout: 0, // 大文件（参考视频）上传不设超时
    headers: { 'Content-Type': undefined }
  })
}

export function getComfyuiTaskList(query) {
  return request({
    url: '/comfyui/task/list',
    method: 'GET',
    params: query
  })
}

export function getComfyuiTaskDetail(id) {
  return request({
    url: `/comfyui/task/detail/${id}`,
    method: 'GET'
  })
}

export function updateComfyuiTask(id, data) {
  return request({
    url: `/comfyui/task/update/${id}`,
    method: 'POST',
    data: data,
    timeout: 0, // 大文件（参考视频）上传不设超时
    headers: { 'Content-Type': undefined }
  })
}

export function translateComfyuiText(data) {
  return request({
    url: '/comfyui/translate',
    method: 'POST',
    data: data
  })
}

export function deleteComfyuiTask(id) {
  return request({
    url: `/comfyui/task/delete/${id}`,
    method: 'POST'
  })
}

export function batchDeleteComfyuiTask(ids) {
  return request({
    url: '/comfyui/task/batch-delete',
    method: 'POST',
    data: { taskIds: ids }
  })
}

export function enqueueComfyuiTask(ids) {
  return request({
    url: '/comfyui/task/enqueue',
    method: 'POST',
    data: { taskIds: ids }
  })
}

export function updateComfyuiPublishStatus(id, publishStatus) {
  return request({
    url: `/comfyui/task/publish-status/${id}`,
    method: 'POST',
    data: { publishStatus }
  })
}

export function batchUpdateComfyuiPublishStatus(ids, publishStatus) {
  return request({
    url: '/comfyui/task/publish-status/batch',
    method: 'POST',
    data: { taskIds: ids, publishStatus }
  })
}

// ===== 执行队列 =====
export function getComfyuiQueueList(query) {
  return request({
    url: '/comfyui/queue/list',
    method: 'GET',
    params: query
  })
}

export function cancelComfyuiQueue(id) {
  return request({
    url: `/comfyui/queue/cancel/${id}`,
    method: 'POST'
  })
}

export function dequeueComfyuiQueue(id) {
  return request({
    url: `/comfyui/queue/dequeue/${id}`,
    method: 'POST'
  })
}

export function retryComfyuiQueue(id) {
  return request({
    url: `/comfyui/queue/retry/${id}`,
    method: 'POST'
  })
}

export function batchRetryComfyuiQueue(ids) {
  return request({
    url: '/comfyui/queue/batch-retry',
    method: 'POST',
    data: { taskIds: ids }
  })
}
