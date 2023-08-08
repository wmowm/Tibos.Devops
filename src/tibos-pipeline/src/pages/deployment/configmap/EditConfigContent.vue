<template>
  <div>
    <a-modal v-model:visible="visible" width="1200px" title="配置信息" @ok="onSubmit">
      <a-spin :spinning="spinning">
        <code-editor ref="codeEditor" v-model="form.content"/>

        <a-form-model ref="ruleForm" :model="form" :label-col="labelCol" :wrapper-col="wrapperCol" :rules="rules">
        <a-form-model-item ref="restartContainer" label="重启容器" prop="restartContainer">
          <a-switch v-model:checked="form.restartContainer" />
        </a-form-model-item>
      </a-form-model>
    </a-spin>
    </a-modal>
  </div>
</template>

<script>


import {updateConfigContent} from '@/services/configmap';
import CodeEditor from '../../components/CodeEditor'



export default {
  name: 'EditConfigContent',
  components: {CodeEditor},
  data () {
    return {
      actionType: 0,
      spinning: false,
      advanced: false,
      visible: false,
      labelCol: { span: 2 },
      wrapperCol: { span: 12 },
      rules:{},
      form:{
        id:0,
        content:{},
        restartContainer:false,
      },
    }
  },
  created:function()
  {
  },
  methods: {
    initData(id,content,restartContainer){
      this.form.id=id
      this.form.content = content==null?'':content
      this.form.restartContainer = restartContainer
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
          let res = await updateConfigContent(this.form)
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
