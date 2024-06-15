<script setup lang="ts">
import { computed } from "vue";
import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";
import { faFilter, faXmark } from "@fortawesome/free-solid-svg-icons";

const open = defineModel<boolean>();

const classes = computed(() => {
    return {
        "translate-closed": !open.value,
        "transform-none": open.value,
    };
});
</script>

<template>
    <div
        :class="classes"
        class="fixed bottom-0 left-0 right-0 z-40 w-full p-3 overflow-y-auto transition-transform bg-white dark:bg-gray-800 border-t-2 border-brand-500"
        tabindex="-1"
        aria-labelledby="drawer-bottom-label"
    >
        <button
            type="button"
            class="text-gray-400 bg-transparent hover:bg-gray-200 hover:text-brand-700 rounded-lg text-sm w-8 h-8 absolute top-2.5 end-2.5 inline-flex items-center justify-center"
            @click="open = !open"
            v-if="open"
        >
            <font-awesome-icon :icon="faXmark" class="h-5 w-5" />
            <span class="sr-only">Close menu</span>
        </button>

        <button
            type="button"
            class="mt-2 text-gray-400 bg-transparent hover:bg-gray-200 hover:text-brand-700 rounded-lg text-sm w-8 h-8 absolute top-2.5 end-2.5 inline-flex items-center justify-center"
            @click="open = !open"
            v-else
        >
            <font-awesome-icon :icon="faFilter" class="h-4 w-4" />
            <span class="sr-only">Open menu</span>
        </button>

        <slot />
    </div>
</template>

<style>
.translate-closed {
    --tw-closed: calc(100% - 4.25rem);
    transform: translateY(var(--tw-closed));
}
</style>
