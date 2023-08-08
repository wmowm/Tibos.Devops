<template>
  <div>
    <a-drawer
      :title="actionType == 0 ? `新增配置`: `编辑配置` "
      :width="480"
      :visible="visible"
      :body-style="{ paddingBottom: '80px' }"
      @close="onClose"
    >

     <a-form-model ref="ruleForm" :model="form" :label-col="labelCol" :wrapper-col="wrapperCol" :rules="rules">
        <a-form-model-item ref="mountPath" label="挂载路径" prop="mountPath">
          <a-input
            placeholder="mountPath"
            v-model="form.mountPath"
            @blur="
              () => {
                $refs.mountPath.onFieldBlur();
              }
            "
          />
        </a-form-model-item>

        <a-form-model-item ref="subPath" label="子路径" prop="subPath">
          <a-input
            placeholder="subPath"
            v-model="form.subPath"
            @blur="
              () => {
                $refs.subPath.onFieldBlur();
              }
            "
          />
        </a-form-model-item>

        <a-form-model-item ref="remark" label="描述" prop="remark">
          <a-textarea
          placeholder="" :default-value="form.remark" v-model="form.remark"
          :auto-size="{ minRows: 2, maxRows: 6 }"
        />
        </a-form-model-item>
      </a-form-model>
       <div
        :style="{
          position: 'absolute',
          right: 0,
          bottom: 0,
          width: '100%',
          borderTop: '1px solid #e9e9e9',
          padding: '10px 16px',
          background: '#fff',
          textAlign: 'right',
          zIndex: 1,
        }"
      >
        <a-button :style="{ marginRight: '8px' }" @click="onClose">
          取消
        </a-button>
        <a-button type="primary" @click="onSubmit">
          提交
        </a-button>
      </div>


    </a-drawer>
  </div>
</template>

<script>

import {getById,createConfigMap,updateConfigMap} from '@/services/configmap';
export default {
  name: 'AddOrEditConfigMap',
  components: {},
  data () {
    return {
      actionType: 0,
      spinning: false,
      advanced: false,
      visible: false,
      labelCol: { span: 6 },
      wrapperCol: { span: 18 },
      form: {
        id: '',
        envId:0,
        appId:0,
        mountPath: '',
        subPath:'',
        remark:'',
      },
      rules:{
        mountPath: [
          { required: true, message: '请输入挂载路径', trigger: 'blur' },
        ],
        subPath: [
          { required: true, message: '请输入子路径', trigger: 'blur' },
        ],
      },
    }
  },
  created:function()
  {
      
  },
  methods: {
    initData(id,envId,appId){
      this.form.id = id
      this.form.envId = envId
      this.form.appId = appId
      if(id=='' || id==null || id== undefined){
      this.form.id=''
      this.form.mountPath = ''
      this.form.subPath = ''
      this.form.remark = ''
      }else{
        getById({id:id}).then(this.afterGetById)  
      }
    },
    showDrawer() {
      this.visible = true;
    },
    onClose() {
      this.visible = false;
    },
    afterGetById(res){
      const data = res.data
      if(data.code == '0'){
        this.form = data.data
      }else{
            this.$message.error(data.message)
        }
    },
    onSubmit () {
      this.$refs.ruleForm.validate(async valid => {
        if (valid) {
          let res = {}
          if(this.actionType == 0){
              res = await createConfigMap(this.form)
          }else{
              res = await updateConfigMap(this.form)
          }
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
  },
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
