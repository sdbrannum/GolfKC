<script setup lang="ts">
interface SelectToggleProps {
    name: string;
    items: string[];
}
const props = defineProps<SelectToggleProps>();
const v = defineModel();

const getId = (item: string) => `${props.name}-${item}`;

const getLabelClasses = (idx: number) => {
    const first = idx === 0;
    const notFirst = idx > 0;
    const last = idx === props.items.length - 1;
    const selected = v.value === props.items[idx];
    return {
        "rounded-l-lg": first,
        "rounded-r-lg": last,
        "border-t": notFirst,
        "border-b": notFirst,
        "border-r": notFirst,
        border: first,
        "border-gray-200": !selected,
        "bg-white": !selected,
        "text-gray-900": !selected,
        "hover:text-rose-700": !selected,
        "hover:bg-gray-100": !selected,
        "hover:bg-rose-700": selected,
        "border-rose-500": selected,
        "bg-rose-500": selected,
        "text-gray-50": selected,
    };
};
</script>

<template>
    <div class="inline-flex rounded-md shadow-sm" role="group">
        <input
            type="radio"
            class="sr-only"
            :name="name"
            v-for="item in items"
            :key="item"
            :id="getId(item)"
            :value="item"
            v-model="v"
        />
        <label
            class="capitalize px-4 py-2 text-sm font-medium dark:bg-gray-800 dark:border-gray-700 dark:text-white dark:hover:text-white dark:hover:bg-gray-700"
            v-for="(item, idx) in items"
            :class="getLabelClasses(idx)"
            :key="item"
            :for="getId(item)"
        >
            {{ item }}
        </label>
    </div>
</template>

<style></style>
