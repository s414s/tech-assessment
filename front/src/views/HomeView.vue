<script setup lang="ts">
import { useUserStore } from '@/stores/userStore';
// import { capitalizeFirstLetter } from '@/helpers/wordUtils';

const { user } = useUserStore();
</script>

<template>
  <div class="title">
    <h1>Home</h1>
    <!-- <p>Bienvenido de nuevo {{ capitalizeFirstLetter(user.name) }} {{ capitalizeFirstLetter(user.surname) }}</p> -->
  </div>
</template>

<style>
.title {
  display: flex;
  flex-direction: column;
  align-items: center;
}
</style>