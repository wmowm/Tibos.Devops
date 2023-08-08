<template>
    <div class="password-strength">
        <div class="password-strength-tip">
            密码强度：<b>{{ strength === 3 ? '高' : (strength === 2 ? '中' : '低') }}</b><span v-show="strength === 2">（推荐）</span>
        </div>
        <ul class="password-strength-bar">
            <li :class="{ active: strength >= 1 }"></li>
            <li :class="{ active: strength >= 2 }"></li>
            <li :class="{ active: strength === 3 }"></li>
        </ul>
    </div>
</template>

<script>
export default {
    name: 'password-strength',
    props: {
        password: {
            type: String,
            default: ''
        }
    },
    data() {
        return {
            strength: 0
        }
    },
    watch: {
        password: {
            handler(newVal) {
                if (!newVal) {
                    this.strength = 0
                    this.$emit('change', this.strength)
                    return
                } else if (newVal.length < 8) {
                    this.strength = 1
                    this.$emit('change', this.strength)
                    return
                }
                let hasNum = 0
                let hasWord = 0
                let hasSpecChar = 0
                for (let i = 0; i < newVal.length; i++) {
                    if (hasNum === 0 && /[0-9]/.test(newVal[i])) {
                        hasNum++
                    } else if (hasWord === 0 && /[a-zA-Z]/.test(newVal[i])) {
                        hasWord++
                    } else if (hasSpecChar === 0 && /[@#￥!^&*()]/.test(newVal[i])) {
                        hasSpecChar++
                    }
                }
                if (hasNum + hasWord + hasSpecChar < 2) {
                    this.strength = 1
                } else if (hasNum + hasWord + hasSpecChar === 2) {
                    this.strength = 2
                } else if (hasNum + hasWord + hasSpecChar === 3) {
                    this.strength = 3
                }
                this.$emit('change', this.strength)
            }
        }
    }
}
</script>

<style lang="less" scoped>
    .password-strength {
        width: 100%;
        margin-bottom: 16px;
        .password-strength-tip {
            display: flex;
            align-items: center;
            margin-bottom: 8px;
            font-size: 12px;
            span {
                color: #666;
            }
        }
        .password-strength-bar {
            padding-left: 0;
            display: flex;
            li {
                flex: 1;
                height: 4px;
                border-radius: 4px;
                opacity: .2;
                list-style-type: none;
                transition: all .2s linear;
                & + li {
                    margin-left: 3px;
                }
                &.active {
                    opacity: 1;
                }
                &:first-child {
                    background: #FF0000;
                }
                &:nth-child(2) {
                    background: #FF7200;
                }
                &:last-child {
                    background: #3FD662;
                }
            }
        }
    }
</style>