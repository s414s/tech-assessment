<script setup lang="ts">
import DateSelect from '@/components/DateSelect.vue'
import UserSelect from '@/components/UserSelect.vue'
import { ApiError, POST } from '@/helpers/apiClient'
import type { CreateBookingResponse, ItemDto } from '@/types/responseTypes'
import { computed, ref, watch } from 'vue'

const props = defineProps<{
  item: ItemDto | null
  date: string // YYYY-MM-DD, initial value for the form
}>()

const emit = defineEmits<{
  created: [id: number]
}>()

const open = defineModel<boolean>({ default: false })

const today = new Date().toLocaleDateString('en-CA')

const userId = ref<number | null>(null)
const date = ref(props.date)
const from = ref('09:00')
const to = ref('10:00')
const saving = ref(false)
const error = ref('')

const canSubmit = computed(() => props.item !== null && userId.value !== null && !!from.value && !!to.value)

// Each opening starts on the date being viewed, without the previous error
watch(open, isOpen => {
  if (!isOpen) return
  date.value = props.date
  error.value = ''
})

async function submit() {
  if (!canSubmit.value || !props.item) return

  saving.value = true
  error.value = ''

  try {
    const response: CreateBookingResponse = await POST('api/bookings', {
      itemId: props.item.id,
      userId: userId.value,
      from: new Date(`${date.value}T${from.value}`).toISOString(),
      to: new Date(`${date.value}T${to.value}`).toISOString(),
    })
    emit('created', response.id)
    open.value = false
  } catch (e) {
    // 4xx responses carry the backend exception message; anything else gets a generic one
    error.value = e instanceof ApiError && e.problem
      ? e.message
      : 'Could not create the booking'
  } finally {
    saving.value = false
  }
}
</script>

<template>
  <v-dialog v-model="open" max-width="500" :persistent="saving">
    <v-card>
      <v-card-title>Book {{ item?.name }}</v-card-title>
      <v-card-subtitle>{{ item?.locationName }}</v-card-subtitle>

      <v-card-text>
        <v-alert
          v-if="error"
          type="error"
          variant="tonal"
          class="dialog-error"
        >
          {{ error }}
        </v-alert>

        <UserSelect v-model="userId" />
        <DateSelect v-model="date" :min="today" />

        <div class="time-fields">
          <v-text-field v-model="from" type="time" label="From" step="900" />
          <v-text-field v-model="to" type="time" label="To" step="900" />
        </div>
      </v-card-text>

      <v-card-actions>
        <v-spacer />
        <v-btn :disabled="saving" @click="open = false">
          Cancel
        </v-btn>
        <v-btn
          color="primary"
          variant="flat"
          :loading="saving"
          :disabled="!canSubmit"
          @click="submit"
        >
          Book
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<style scoped>
.dialog-error {
  margin-bottom: 16px;
}

.time-fields {
  display: flex;
  gap: 16px;
}
</style>
