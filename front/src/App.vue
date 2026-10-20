<script setup lang="ts">
import { RouterView } from 'vue-router';

import Header from './components/Header.vue';
// import Footer from './components/Footer.vue';
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
    width: 100vw;
    height: 100vh;
}

.content {
    flex-grow: 1;
    width: 100%;
}
</style>