<template>
  <div>
    <el-row :gutter="15">
      <el-form ref="elForm" :model="formData" :rules="rules" size="medium" label-width="100px">
        <el-col :span="12">
          <el-form-item label="时间范围" prop="field101">
            <el-time-picker v-model="formData.field101" is-range format="HH:mm:ss" value-format="HH:mm:ss"
              :style="{width: '100%'}" start-placeholder="开始时间" end-placeholder="结束时间" range-separator="至"
              clearable></el-time-picker>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="商户名称" prop="field102">
            <el-input v-model="formData.field102" placeholder="请输入商户名称" clearable :style="{width: '100%'}">
            </el-input>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="运单号" prop="field103">
            <el-input v-model="formData.field103" placeholder="请输入运单号" clearable :style="{width: '100%'}">
            </el-input>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="状态" prop="field104">
            <el-select v-model="formData.field104" placeholder="请选择状态" clearable :style="{width: '100%'}">
              <el-option v-for="(item, index) in field104Options" :key="index" :label="item.label"
                :value="item.value" :disabled="item.disabled"></el-option>
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="状态" prop="field105">
            <el-select v-model="formData.field105" placeholder="请选择状态" clearable :style="{width: '100%'}">
              <el-option v-for="(item, index) in field105Options" :key="index" :label="item.label"
                :value="item.value" :disabled="item.disabled"></el-option>
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="问题件类型" prop="field106">
            <el-select v-model="formData.field106" placeholder="请选择问题件类型" clearable :style="{width: '100%'}">
              <el-option v-for="(item, index) in field106Options" :key="index" :label="item.label"
                :value="item.value" :disabled="item.disabled"></el-option>
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="问题件类别" prop="field107">
            <el-select v-model="formData.field107" placeholder="请选择问题件类别" clearable :style="{width: '100%'}">
              <el-option v-for="(item, index) in field107Options" :key="index" :label="item.label"
                :value="item.value" :disabled="item.disabled"></el-option>
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="6">
          <el-form-item label="查询" prop="field108">
            <el-button type="primary" icon="el-icon-search" size="medium"> 查询 </el-button>
          </el-form-item>
        </el-col>
        <el-col :span="6">
          <el-form-item label="转发商户" prop="field109">
            <el-button type="primary" icon="el-icon-search" size="medium"> 转发商户 </el-button>
          </el-form-item>
        </el-col>
        <el-col :span="24">
          <el-row gutter="15">
          </el-row>
        </el-col>
        <el-col :span="24">
          <el-form-item size="large">
            <el-button type="primary" @click="submitForm">提交</el-button>
            <el-button @click="resetForm">重置</el-button>
          </el-form-item>
        </el-col>
      </el-form>
    </el-row>
  </div>
</template>
<script>
export default {
  components: {},
  props: [],
  data() {
    return {
      formData: {
        field101: null,
        field102: undefined,
        field103: undefined,
        field104: undefined,
        field105: undefined,
        field106: undefined,
        field107: undefined,
        field108: undefined,
        field109: undefined,
      },
      rules: {
        field101: [{
          required: true,
          message: '时间范围不能为空',
          trigger: 'change'
        }],
        field102: [{
          required: true,
          message: '请输入商户名称',
          trigger: 'blur'
        }],
        field103: [{
          required: true,
          message: '请输入运单号',
          trigger: 'blur'
        }],
        field104: [{
          required: true,
          message: '请选择状态',
          trigger: 'change'
        }],
        field105: [{
          required: true,
          message: '请选择状态',
          trigger: 'change'
        }],
        field106: [{
          required: true,
          message: '请选择问题件类型',
          trigger: 'change'
        }],
        field107: [{
          required: true,
          message: '请选择问题件类别',
          trigger: 'change'
        }],
      },
      field104Options: [{
        "label": "选项一",
        "value": 1
      }, {
        "label": "选项二",
        "value": 2
      }],
      field105Options: [{
        "label": "选项一",
        "value": 1
      }, {
        "label": "选项二",
        "value": 2
      }],
      field106Options: [{
        "label": "选项一",
        "value": 1
      }, {
        "label": "选项二",
        "value": 2
      }],
      field107Options: [{
        "label": "选项一",
        "value": 1
      }, {
        "label": "选项二",
        "value": 2
      }],
    }
  },
  computed: {},
  watch: {},
  created() {},
  mounted() {},
  methods: {
    submitForm() {
      this.$refs['elForm'].validate(valid => {
        if (!valid) return
        // TODO 提交表单
      })
    },
    resetForm() {
      this.$refs['elForm'].resetFields()
    },
  }
}

</script>
<style>
</style>
