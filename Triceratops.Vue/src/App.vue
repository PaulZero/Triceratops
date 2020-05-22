<template>
    <layout />  
</template>

<script lang="ts">
    import { Component, Vue } from 'vue-property-decorator';
    import Layout from '@/views/Layout.vue';

    @Component({
        components: {
            Layout
        }
    })
    export default class App extends Vue {

    }
</script>

<style>
</style>
