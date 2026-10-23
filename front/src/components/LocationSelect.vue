<script setup lang="ts">
import { GET } from '@/helpers/apiClient'
import type { Location } from '@/types/responseTypes'
import { onMounted, ref } from 'vue'

const model = defineModel<number | null>({ default: null })

const locations = ref<Location[]>([])
const loading = ref(true)
const error = ref('')

onMounted(async () => {
  try {
    locations.value = await GET('api/locations')
  } catch {
    error.value = 'Could not load locations'
  } finally {
    loading.value = false
  }
})
</script>

<template>
  <v-select
    v-model="model"
    :items="locations"
    item-title="name"
    item-value="id"
    label="Location"
    :loading="loading"
    :error-messages="error"
    clearable
  />
</template>
