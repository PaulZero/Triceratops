<template>
    <span>{{ message }}</span>
</template>

<script lang="ts">
    import { Component, Vue } from 'vue-property-decorator'
    @Component
    export default class Home extends Vue {       
        private message: string = 'hello, everybody!';
    }
</script>

<style scoped>
</style>