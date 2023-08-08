<template>
  <div>
    <a-modal v-model:visible="visible" width="400px" title="回滚部署" @ok="onSubmit">
      <a-spin :spinning="spinning">
        <a-form-model ref="ruleForm" :model="form" :label-col="labelCol" :wrapper-col="wrapperCol" :rules="rules">
            <a-form-model-item ref="remark" label="描述" prop="remark">
              <a-textarea
              placeholder="" :default-value="form.remark" v-model="form.remark"
              :auto-size="{ minRows: 3, maxRows: 6 }"
            />
            </a-form-model-item>
          </a-form-model>
      </a-spin>
    </a-modal>
  </div>
</template>

<script>

import {rollBackPublish} from '@/services/deployment';


export default {
  name: 'RollBackPublish',
  data () {
    return {
      spinning: false,
      advanced: false,
      visible: false,
      labelCol: { span: 4 },
      wrapperCol: { span: 20 },
      form:{
        id:0,
        remark:''
      },
      rules:{},
    }
  },
  created:function()
  {
   
  },
  methods: {
    initData(id){
      this.form.id = id
      this.form.remark=''
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
          let res = await rollBackPublish(this.form)
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
