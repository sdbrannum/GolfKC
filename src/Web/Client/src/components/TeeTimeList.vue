<script setup lang="ts">
import { ref, watch, computed } from "vue";
import TeeTimeCard from "./TeeTimeCard.vue";
import { getTimes } from "../api";
import TeeTimeCardSkeleton from "./TeeTimeCardSkeleton.vue";

const props = defineProps<{
    source: string;
    courseId: string;
    date: string | null;
    players: string;
    holes: string;
}>();

const loading = ref(false);
const times = ref<TeeTime[]>([]);
const filteredTimes = computed(() => {
    return times.value.filter((time) => {
        return (
            (props.holes === "any" || time.holes.toString() === props.holes) &&
            (props.players === "any" ||
                time.players.toString() === props.players)
        );
    });
});

const loadTeeTimes = async () => {
    if (!props.date) {
        return;
    }
    try {
        loading.value = true;
        times.value = await getTimes(props.source, props.courseId, props.date);
    } catch {
    } finally {
        loading.value = false;
    }
};

watch(() => props.date, loadTeeTimes, { immediate: true });

const getRandom = (min: number, max: number) => {
    return Math.floor(Math.random() * (max - min + 1) + min);
};

const getTeeTimeKey = (teeTime: TeeTime) => {
    return `${props.courseId}:${teeTime.time}`;
};
</script>

<template>
    <ol class="py-3 px-4 -ml-4 flex overflow-x-auto">
        <template v-if="loading">
            <tee-time-card-skeleton
                class="mr-1"
                v-for="n in getRandom(1, 10)"
                :key="n"
            />
        </template>
        <template v-else-if="filteredTimes.length">
            <tee-time-card
                class="mr-1"
                v-for="teeTime in filteredTimes"
                :key="getTeeTimeKey(teeTime)"
                v-bind="teeTime"
            />
        </template>
        <template v-else>
            <li>
                <span>No tee times match your criteria</span>
                <span class="text-sm block text-gray-300"
                    >Course may not allow booking past 5-7 days out</span
                >
            </li>
        </template>
    </ol>
</template>

<style scoped>
ol {
    width: calc(100% + 2rem);
}
</style>
