<script setup lang="ts">
import DateSelect from '@/components/DateSelect.vue'
import ItemSelect from '@/components/ItemSelect.vue'
import LocationSelect from '@/components/LocationSelect.vue'
import UserSelect from '@/components/UserSelect.vue'
import { ApiError, POST } from '@/helpers/apiClient'
import type { CreateBookingResponse, ItemDto } from '@/types/responseTypes'
import { computed, ref, watch } from 'vue'

const props = defineProps<{
  item: ItemDto | null // Preselected item; null lets the user pick location and item
  date: string // YYYY-MM-DD, initial value for the form
}>()

const emit = defineEmits<{
  created: [id: number]
}>()

const open = defineModel<boolean>({ default: false })

const today = new Date().toLocaleDateString('en-CA')

const locationId = ref<number | null>(null)
const itemId = ref<number | null>(null)
const userId = ref<number | null>(null)
const date = ref(props.date)
const from = ref('09:00')
const to = ref('10:00')
const saving = ref(false)
const error = ref('')

const canSubmit = computed(() => itemId.value !== null && userId.value !== null && !!from.value && !!to.value)

// Each opening starts on the date being viewed and the preselected item (if any), without the previous error
watch(open, isOpen => {
  if (!isOpen) return
  locationId.value = props.item?.locationId ?? null
  itemId.value = props.item?.id ?? null
  date.value = props.date
  error.value = ''
})

async function submit() {
  if (!canSubmit.value) return

  saving.value = true
  error.value = ''

  try {
    const response: CreateBookingResponse = await POST('api/bookings', {
      itemId: itemId.value,
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
      <v-card-title>New booking</v-card-title>

      <v-card-text>
        <v-alert
          v-if="error"
          type="error"
          variant="tonal"
          class="dialog-error"
        >
          {{ error }}
        </v-alert>

        <LocationSelect v-model="locationId" />
        <ItemSelect v-model="itemId" :location-id="locationId" />
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
