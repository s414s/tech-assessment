<script setup lang="ts">
import { GET } from '@/helpers/apiClient'
import type { User } from '@/types/responseTypes'
import { onMounted, ref } from 'vue'

const model = defineModel<number | null>({ default: null })

const users = ref<User[]>([])
const loading = ref(true)
const error = ref('')

onMounted(async () => {
  try {
    users.value = await GET('api/users')
  } catch {
    error.value = 'Could not load users'
  } finally {
    loading.value = false
  }
})
</script>

<template>
  <v-select
    v-model="model"
    :items="users"
    item-title="name"
    item-value="id"
    label="User"
    :loading="loading"
    :error-messages="error"
    clearable
  />
</template>
