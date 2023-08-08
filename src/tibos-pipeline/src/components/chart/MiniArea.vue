<template>
  <div class="mini-chart">
    <div class="chart-content" :style="{height: 46}">
      <v-chart :force-fit="true" :height="height"  :data="data" :padding="[36, 5, 18, 5]">
        <v-tooltip />
        <v-smooth-area position="x*y" />
      </v-chart>
    </div>
  </div>
</template>

<script>



const tooltip = [
  'x*y',
  (x, y) => ({
    name: x,
    value: y
  })
]


export default {
  name: 'MiniArea',
  props: {
    data: Array
  },
  data () {
    return {
      tooltip:tooltip,
      height: 100
    }
  }
}
</script>

<style scoped>
  .mini-chart {
    position: relative;
    width: 100%
  }
  .mini-chart .chart-content{
    position: absolute;
    bottom: -28px;
    width: 100%;
  }
</style>
