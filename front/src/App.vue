<script setup lang="ts">
import { RouterView } from 'vue-router';

import Header from './components/Header.vue';
</script>

<template>
    <div class="main-container">
        <Header />
        <main class="content">
            <RouterView />
        </main>
        <Footer />
    </div>
</template>

<style scoped>
.main-container {
    display: flex;
    flex-direction: column;
    align-items: center;
    width: 100%;
    min-height: 100vh;
}

.content {
    flex-grow: 1;
    width: 100%;
    padding: 20px
}
</style>