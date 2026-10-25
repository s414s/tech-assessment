<script setup lang="ts">
import { formatDate, isWeekend } from '@/helpers/dateHelpers'
import { computed, ref } from 'vue'

// YYYY-MM-DD
const model = defineModel<string>({ required: true })

defineProps<{
  min?: string // YYYY-MM-DD, earliest selectable date
}>()

const menu = ref(false)

const pickerDate = computed<Date>({
  get: () => new Date(`${model.value}T12:00:00`),
  set: (value: Date) => {
    model.value = value.toLocaleDateString('en-CA')
    menu.value = false
  },
})

// Vuetify passes the picker's own date value; a plain YYYY-MM-DD string is read as local noon
function isAllowed(value: unknown): boolean {
  const date = typeof value === 'string' ? new Date(`${value}T12:00:00`) : new Date(value as Date)
  return !isWeekend(date)
}
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

    <v-date-picker v-model="pickerDate" :min="min" :allowed-dates="isAllowed" />
  </v-menu>
</template>
