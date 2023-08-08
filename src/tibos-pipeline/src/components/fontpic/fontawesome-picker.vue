<template>
  <div>
    <a-popover placement="bottom" v-model="fontVisible">
      <template v-slot:content>
        <a-input-search
          placeholder="搜索图标"
          v-model="fontSearchValue"
          @input="fontSearch(fontSearchValue)"
        />
        <div :style="{ width: '260px', height: '260px', overflow: 'auto' }">
          <div class="font-container">
            <div
              v-for="item in showJson"
              :key="item"
              @click="fontClick(item)"
              class="my-font"
            >
             <a-icon :type= item />
            </div>
          </div>
        </div>
      </template>
      <template v-slot:title >
       
      </template>
      <a-input
        :read-only="true"
        :value="value"
        @mouseover="mouseOver"
        :placeholder="placeholder"
      >
        <template v-slot:addonAfter>
          <a>
            <div v-if="value">
              <a-icon :type="value" />
            </div>
            <div v-else>
              <a-icon type="file-search" />
            </div>
          </a>
        </template>
      </a-input>
    </a-popover>
  </div>
</template>
 
<script>
import fontJson from "./fontJson.json";
import Popover from "ant-design-vue/lib/popover";
import Input from "ant-design-vue/lib/input";
import InputSearch from "ant-design-vue/lib/input/Search";
import "ant-design-vue/dist/antd.min.css";


export default {
  name: "fontawesomePicker",
  data() {
    return {
      showJson: null,
      fontVisible: false,
      value:'',
      fontSearchValue: null
    };
  },
  components: {
    "a-popover": Popover,
    "a-input": Input,
    "a-input-search": InputSearch,
  },
  model: {
    event: "change"
  },
  props: {
    placeholder: {
      type: String,
      default: null
    }
  },
  methods: {
    mouseOver() {
      this.fontSearchValue = null;
      this.showJson = fontJson;
    },
    fontSearch(value) {
          console.log(value)
      if (value) {
        this.showJson = fontJson.filter(item => {
          return item.startsWith(value);
        });
      } else {
        this.showJson = fontJson;
      }
    },
    fontClick(item) {
      this.$emit("change", item);
      this.fontVisible = false;
      console.log(item)
      this.value = item
      
    }
  }
};
</script>
 
<style scoped>
.font-container {
  font-size: 18px;
  width: 235px;
  height: 235px;
  display: flex;
  flex-wrap: wrap;
  flex-direction: row;
  align-content: flex-start;
  justify-content: center;
}
.my-font {
  cursor: pointer;
  margin: 5px 5px 5px 5px;
  padding-top: 5px;
  text-align: center;
  border-radius: 6px;
  width: 45px;
  height: 40px;
  border: 1px solid #ccc;
}
.my-font:hover {
  background-color: rgba(206, 206, 206, 0.5);
}
</style>