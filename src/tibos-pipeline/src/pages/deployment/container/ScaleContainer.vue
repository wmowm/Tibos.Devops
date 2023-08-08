<template>
  <div>
    <a-modal v-model:visible="visible" width="450px" title="伸缩" @ok="onSubmit">
      <a-spin :spinning="spinning">
        <a-form-model ref="ruleForm" :model="form" :label-col="labelCol" :wrapper-col="wrapperCol" :rules="rules">
            <a-form-model-item label="容器数量" prop='replicas'>
            <a-select  
                      v-model="form.replicas"                 
                      placeholder="请选择容器数量"
            >
            <a-select-option v-for="item in this.options" :key="item">
            {{item}}
            </a-select-option>
          </a-select>
        </a-form-model-item>

          </a-form-model>
      </a-spin>
    </a-modal>
  </div>
</template>

<script>

import {scalePod} from '@/services/container';

const options = [...Array(10)].map((_, i) => (i+1));
console.log(options)
export default {
  name: 'ScaleContainer',
  data () {
    return {
      options:options,
      spinning: false,
      advanced: false,
      visible: false,
      labelCol: { span: 4 },
      wrapperCol: { span: 20 },
      form:{
        appId:0,
        envId:0,
        replicas:0
      },
      rules:{},
    }
  },
  created:function()
  {
   
  },
  methods: {
    initData(envId,appId,replicas){
      this.form.appId = appId
      this.form.envId=envId
      this.form.replicas=replicas
    },
    showDrawer() {
      this.visible = true;
    },
    onClose() {
      this.visible = false;
    },
    onSubmit () {
      this.$refs.ruleForm.validate(async valid => {
        if (valid) {
          let res = await scalePod(this.form)
          const data = res.data;
          if (data.code === '0') {
            this.spinning = false
            this.$message.success('操作成功!');
            this.$emit('refresh');
            this.onClose()

          } else {
            this.spinning = false
            this.$message.error(data.message)
          }
        } else {
          return false
        }
      })
    },
    toggleAdvanced () {
      this.advanced = !this.advanced
    },
  }
}
</script>

<style lang="less" scoped>
  .search{
    margin-bottom: 54px;
  }
  .fold{
    width: calc(100% - 216px);
    display: inline-block
  }
  .operator{
    margin-bottom: 18px;
  }
  @media screen and (max-width: 900px) {
    .fold {
      width: 100%;
    }
  }
</style>
