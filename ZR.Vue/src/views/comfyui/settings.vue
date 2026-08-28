<template>
  <div class="app-container">
    <el-card class="box-card">
      <div slot="header" class="clearfix">
        <span>ComfyUI 服务设置</span>
      </div>

      <el-form ref="form" :model="form" :rules="rules" label-width="120px" class="form-left">
        <el-form-item label="ComfyUI地址" prop="serverUrl">
          <el-input v-model="form.serverUrl" placeholder="如 http://127.0.0.1:8188 或 http://192.168.1.100:8188">
            <template slot="prepend">Server URL</template>
          </el-input>
          <div class="tip">ComfyUI服务端地址（域名/IP:端口）。参考图会上传到该服务器的 input 目录，任务执行也提交到此服务。</div>
        </el-form-item>

        <el-form-item>
          <el-button type="primary" icon="el-icon-cpu" @click="handleTest" :loading="testing">测试连接</el-button>
          <el-button type="success" icon="el-icon-check" @click="handleSave" :loading="saving">保存</el-button>
        </el-form-item>

        <el-alert v-if="testResult" :title="testResult.message" :type="testResult.ok ? 'success' : 'error'" show-icon :closable="false" style="margin-top:8px" />
      </el-form>
    </el-card>
  </div>
</template>

<script>
import { getComfyuiConfig, saveComfyuiConfig, testComfyuiConfig } from '@/api/comfyui/index'

export default {
  name: 'ComfyuiSettings',
  data() {
    return {
      form: { serverUrl: '' },
      rules: {
        serverUrl: [{ required: true, message: '请输入ComfyUI地址', trigger: 'blur' }]
      },
      testing: false,
      saving: false,
      testResult: null
    }
  },
  created() { this.loadConfig() },
  methods: {
    loadConfig() {
      getComfyuiConfig().then(res => {
        this.form.serverUrl = res.data.serverUrl || ''
      })
    },
    handleTest() {
      if (!this.form.serverUrl) { this.$message.warning('请先输入ComfyUI地址'); return }
      this.testing = true
      this.testResult = null
      testComfyuiConfig({ serverUrl: this.form.serverUrl }).then(res => {
        this.testResult = { ok: res.data.ok, message: res.data.message }
        if (res.data.ok) this.$message.success(res.data.message)
        else this.$message.error(res.data.message)
      }).catch(() => { this.$message.error('测试请求失败') }).finally(() => { this.testing = false })
    },
    handleSave() {
      this.$refs.form.validate(valid => {
        if (!valid) return
        this.saving = true
        saveComfyuiConfig({ serverUrl: this.form.serverUrl }).then(res => {
          this.$message.success(res.data.message || '保存成功')
          this.testResult = null
        }).catch(() => { this.$message.error('保存失败') }).finally(() => { this.saving = false })
      })
    }
  }
}
</script>

<style scoped>
.form-left { max-width: 640px; }
.tip { font-size: 12px; color: #909399; line-height: 1.5; margin-top: 4px; }
</style>
