<template>
    <span @click="init()">{{ message }}</span>
</template>

<script lang="ts">
    import { Component, Vue } from 'vue-property-decorator'
    import { getModule } from 'vuex-module-decorators';
    import ApiStore from '../ts/stores/api/ApiStore';
    import { Getter } from 'vuex-class';

    @Component
    export default class Home extends Vue {
        @Getter('name')
        private message;

        init(): void {
            this.$store.dispatch('activateBarry');
        }
    }
</script>

<style scoped>
</style>