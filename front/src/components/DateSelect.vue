<script setup lang="ts">
import { formatDate } from '@/helpers/dateHelpers'
import { computed, ref } from 'vue'

// YYYY-MM-DD
const model = defineModel<string>({ required: true })

const menu = ref(false)

const pickerDate = computed<Date>({
  get: () => new Date(`${model.value}T12:00:00`),
  set: (value: Date) => {
    model.value = value.toLocaleDateString('en-CA')
    menu.value = false
  },
})
</script>

<template>
  <v-menu
    v-model="menu"
    :close-on-content-click="false"
  >
    <template #activator="{ props }">
      <v-text-field
        v-bind="props"
        :model-value="formatDate(model)"
        label="Date"
        readonly
      />
    </template>

    <v-date-picker v-model="pickerDate" />
  </v-menu>
</template>
