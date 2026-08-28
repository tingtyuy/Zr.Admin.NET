<template>
  <CreateTask func-type="txt2img" title="文生图" />
</template>

<script>
import CreateTask from './components/CreateTask.vue'
export default {
  name: 'ComfyuiTxt2Img',
  components: { CreateTask }
}
</script>
