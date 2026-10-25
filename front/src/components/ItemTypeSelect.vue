<script setup lang="ts">
import { GET } from '@/helpers/apiClient'
import type { ItemType } from '@/types/responseTypes'
import { onMounted, ref } from 'vue'

const model = defineModel<number | null>({ default: null })

const types = ref<ItemType[]>([])
const loading = ref(true)
const error = ref('')

onMounted(async () => {
  try {
    types.value = await GET('api/items/types')
  } catch {
    error.value = 'Could not load item types'
  } finally {
    loading.value = false
  }
})
</script>

<template>
  <v-select
    v-model="model"
    :items="types"
    item-title="name"
    item-value="id"
    label="Item type"
    :loading="loading"
    :error-messages="error"
    clearable
  />
</template>
